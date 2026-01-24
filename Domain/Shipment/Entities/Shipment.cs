using Domain.Common;
using Domain.Shipment.Enums;
using Domain.Shipment.Events;
using Domain.Shipment.ValueObjects;
using Domain.Shipment.Rules; // Assuming you have business rules engine

namespace Domain.Shipment.Entities
{
    public class Shipment : AggregateRoot
    {
        public string TrackingNumber { get; private set; }
        public ShipmentStatus Status { get; private set; }
        public Route Route { get; private set; }
        public DateTime? DispatchedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? EstimatedDeliveryDate { get; private set; }

        // Vehicle Assignment (Optional - للربط مع Fleet Domain)
        public Guid? AssignedVehicleId { get; private set; }

        // Encapsulation: Collections
        private readonly List<Package> _packages = new();
        public virtual IReadOnlyCollection<Package> Packages => _packages.AsReadOnly();

        private readonly List<TrackingUpdate> _trackingUpdates = new();
        public virtual IReadOnlyCollection<TrackingUpdate> TrackingUpdates => _trackingUpdates.AsReadOnly();

        // ============================================
        // CONSTRUCTORS
        // ============================================
        private Shipment() { } // EF Core

        public Shipment(string trackingNumber, Route route, DateTime? estimatedDeliveryDate = null)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentNullException(nameof(trackingNumber));

            TrackingNumber = trackingNumber;
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Status = ShipmentStatus.Created;
            EstimatedDeliveryDate = estimatedDeliveryDate ?? DateTime.UtcNow.AddDays(3);

            AddDomainEvent(new ShipmentCreatedEvent(Id, TrackingNumber));
        }

        // ============================================
        // BUSINESS LOGIC: Package Management
        // ============================================
        public void AddPackage(double weight, string description, PackageType type, decimal declaredValue)
        {
            CheckRule(new ShipmentCannotBeModifiedAfterDispatchRule(Status));

            var package = new Package(Id, weight, description, type, declaredValue);
            _packages.Add(package);

            // Check weight limit
            CheckRule(new TotalWeightMustNotExceedLimitRule(GetTotalWeight()));

            AddDomainEvent(new PackageAddedToShipmentEvent(Id, package.Id));
        }

        public void RemovePackage(Guid packageId)
        {
            CheckRule(new ShipmentCannotBeModifiedAfterDispatchRule(Status));

            var package = _packages.FirstOrDefault(p => p.Id == packageId);
            if (package == null)
                throw new ArgumentException("Package not found", nameof(packageId));

            _packages.Remove(package);
        }

        // ============================================
        // BUSINESS LOGIC: Shipment Lifecycle
        // ============================================
        public void AssignVehicle(Guid vehicleId)
        {
            CheckRule(new ShipmentCannotBeModifiedAfterDispatchRule(Status));

            AssignedVehicleId = vehicleId;
        }

        public void StartJourney()
        {
            CheckRule(new ShipmentMustHavePackagesRule(_packages.Count));

            if (Status != ShipmentStatus.Created)
                return; // Idempotency

            Status = ShipmentStatus.InTransit;
            DispatchedAt = DateTime.UtcNow;

            AddTrackingUpdate(Route.Origin.City, "Shipment dispatched from origin facility");
            AddDomainEvent(new ShipmentDispatchedEvent(Id));
            AddDomainEvent(new ShipmentStatusChangedEvent(Id, Status));
        }

        public void MarkAsOutForDelivery()
        {
            if (Status != ShipmentStatus.InTransit)
                throw new InvalidOperationException("Shipment must be in transit");

            Status = ShipmentStatus.OutForDelivery;
            AddTrackingUpdate(Route.Destination.City, "Out for delivery");
            AddDomainEvent(new ShipmentStatusChangedEvent(Id, Status));
        }

        public void CompleteDelivery()
        {
            CheckRule(new ShipmentMustBeInTransitForDeliveryRule(Status));
            CheckRule(new ShipmentCannotBeDeliveredTwiceRule(Status));

            Status = ShipmentStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;

            AddTrackingUpdate(Route.Destination.GetFullAddress(), "Delivered to recipient");
            AddDomainEvent(new ShipmentDeliveredEvent(Id, DeliveredAt.Value));
            AddDomainEvent(new ShipmentStatusChangedEvent(Id, Status));
        }

        public void Cancel(string reason)
        {
            if (Status == ShipmentStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel delivered shipment");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = ShipmentStatus.Cancelled;
            AddTrackingUpdate("Cancelled", reason);
            AddDomainEvent(new ShipmentCancelledEvent(Id, reason));
            AddDomainEvent(new ShipmentStatusChangedEvent(Id, Status));
        }

        public void MarkAsReturned(string reason)
        {
            if (Status != ShipmentStatus.InTransit && Status != ShipmentStatus.OutForDelivery)
                throw new InvalidOperationException("Can only return shipments that are in transit");

            Status = ShipmentStatus.Returned;
            AddTrackingUpdate("Returning to sender", reason);
            AddDomainEvent(new ShipmentStatusChangedEvent(Id, Status));
        }

        // ============================================
        // BUSINESS LOGIC: Tracking
        // ============================================
        public void RecordLocation(string location, string? notes = null)
        {
            CheckRule(new CannotAddTrackingToFinalizedShipmentRule(Status));
            AddTrackingUpdate(location, notes);
        }

        private void AddTrackingUpdate(string location, string? notes = null)
        {
            var update = new TrackingUpdate(Id, location, notes);
            _trackingUpdates.Add(update);
            AddDomainEvent(new TrackingUpdateAddedEvent(Id, location));
        }

        // ============================================
        // CALCULATED PROPERTIES & QUERIES
        // ============================================
        public double GetTotalWeight() => _packages.Sum(p => p.Weight);

        public decimal GetTotalDeclaredValue() => _packages.Sum(p => p.DeclaredValue);

        public int GetPackageCount() => _packages.Count;

        public bool IsDelayed() => EstimatedDeliveryDate.HasValue &&
                                   DateTime.UtcNow > EstimatedDeliveryDate.Value &&
                                   Status != ShipmentStatus.Delivered;

        public bool HasSpecialHandlingPackages() => _packages.Any(p => p.RequiresSpecialHandling());

        public TrackingUpdate? GetLatestTrackingUpdate() => _trackingUpdates.OrderByDescending(t => t.Timestamp).FirstOrDefault();

        public bool CanBeModified() => Status == ShipmentStatus.Created;

        public bool IsInProgress() => Status == ShipmentStatus.InTransit || Status == ShipmentStatus.OutForDelivery;

        public bool IsFinalized() => Status == ShipmentStatus.Delivered ||
                                      Status == ShipmentStatus.Cancelled ||
                                      Status == ShipmentStatus.Returned;
    }
}