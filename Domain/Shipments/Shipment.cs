using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using Domain.Shipments.Errors;
using Domain.Shipments.Events;
using Domain.Shipments.Rules;
using Domain.Shipments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments
{
    public sealed class Shipment: AggregateRoot<ShipmentId>
    {
        // ─── Core Fields ─────────────────────────────────────────
        public string SenderId { get; private set; } = default!;
        public string? RecipientName { get; private set; }
        public string? RecipientPhone { get; private set; }
        public DeliveryAddress DestinationAddress { get; private set; } = default!;
        public TrackingInfo Tracking { get; private set; } = default!;
        public ShipmentPriority Priority { get; private set; }
        public int Version { get; private set; } = 0;

        // ─── Packages (Entity Collection, only via AR) ───────────
        private readonly List<Package> _packages = new();
        public IReadOnlyList<Package> Packages => _packages.AsReadOnly();

        // ─── Route (List<RoutePoint> VOs, NOT a Route entity) ────
        private readonly List<RoutePoint> _route = new();
        public IReadOnlyList<RoutePoint> Route => _route.AsReadOnly();

        // ─── Audit ───────────────────────────────────────────────
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? SpecialInstructions { get; private set; }

        // EF Core
        private Shipment() { }

        // ─── Factory ─────────────────────────────────────────────

        public static Result<Shipment> Create(
            string senderId,
            DeliveryAddress destinationAddress,
            string trackingNumber,
            ShipmentPriority priority = ShipmentPriority.Standard,
            string? recipientName = null,
            string? recipientPhone = null,
            string? specialInstructions = null)
        {
            if (string.IsNullOrWhiteSpace(senderId))
                return Result<Shipment>.Failure(ShipmentErrors.EmptySenderId);

            var shipment = new Shipment
            {
                Id = ShipmentId.New(),
                SenderId = senderId.Trim(),
                DestinationAddress = destinationAddress,
                Priority = priority,
                RecipientName = recipientName?.Trim(),
                RecipientPhone = recipientPhone?.Trim(),
                SpecialInstructions = specialInstructions?.Trim(),
                CreatedAt = DateTime.UtcNow,
                Tracking = TrackingInfo.Create(trackingNumber, ShipmentStatus.Created, "Shipment created and awaiting packages.")
            };

            shipment.AddDomainEvent(new ShipmentCreatedEvent(
                shipment.Id,
                shipment.SenderId,
                shipment.DestinationAddress,
                trackingNumber));

            return Result<Shipment>.Success(shipment);
        }

        // ─── Package Management ───────────────────────────────────

        public Result<PackageId> AddPackage(
            string description,
            Weight weight,
            Dimensions dimensions,
            string? contentCategory = null,
            bool isFragile = false,
            bool requiresRefrigeration = false,
            decimal declaredValue = 0,
            string currency = "USD")
        {
            var terminalCheck = CheckRule(new ShipmentMustNotBeTerminalRule(Tracking));
            if (terminalCheck.IsFailure) return Result<PackageId>.Failure(terminalCheck.Error);

            var cancelledCheck = CheckRule(new ShipmentMustNotBeCancelledRule(Tracking.Status));
            if (cancelledCheck.IsFailure) return Result<PackageId>.Failure(cancelledCheck.Error);

            var packageResult = Package.Create(
                description, weight, dimensions,
                contentCategory, isFragile, requiresRefrigeration,
                declaredValue, currency);

            if (packageResult.IsFailure) return Result<PackageId>.Failure(packageResult.Error);

            _packages.Add(packageResult.Value);
            Touch();

            AddDomainEvent(new PackageAddedToShipmentEvent(
                Id, packageResult.Value.Id, packageResult.Value.Description, packageResult.Value.Weight.ToKilograms()));

            return Result<PackageId>.Success(packageResult.Value.Id);
        }

        public Result RemovePackage(PackageId packageId)
        {
            var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
            if (editableCheck.IsFailure) return editableCheck;

            var package = _packages.FirstOrDefault(p => p.Id == packageId);
            if (package is null)
                return Result.Failure(ShipmentErrors.PackageNotFound);

            _packages.Remove(package);
            Touch();

            AddDomainEvent(new PackageRemovedFromShipmentEvent(Id, packageId));
            return Result.Success();
        }

        // ─── Carrier & Dispatch ──────────────────────────────────

        public Result AssignCarrier(string carrierName, string? estimatedDeliveryDate = null)
        {
            var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
            if (editableCheck.IsFailure) return editableCheck;

            Tracking = Tracking.WithCarrier(carrierName, estimatedDeliveryDate);
            Touch();

            AddDomainEvent(new CarrierAssignedEvent(Id, carrierName, estimatedDeliveryDate));
            return Result.Success();
        }

        public Result Dispatch()
        {
            var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
            if (editableCheck.IsFailure) return editableCheck;

            var packagesCheck = CheckRule(new ShipmentMustHavePackagesRule(_packages.Count));
            if (packagesCheck.IsFailure) return packagesCheck;

            var carrierCheck = CheckRule(new ShipmentMustHaveCarrierRule(Tracking.CarrierName));
            if (carrierCheck.IsFailure) return carrierCheck;

            Tracking = Tracking.WithStatus(ShipmentStatus.InTransit, "Shipment dispatched and in transit.");
            Touch();

            AddDomainEvent(new ShipmentDispatchedEvent(Id, Tracking.TrackingNumber, Tracking.CarrierName!));
            return Result.Success();
        }

        // ─── Route Management ─────────────────────────────────────

        public Result AddRoutePoint(string location, string description, DateTime arrivedAt, RoutePointType type)
        {
            var cancelledCheck = CheckRule(new ShipmentMustNotBeCancelledRule(Tracking.Status));
            if (cancelledCheck.IsFailure) return cancelledCheck;

            var terminalCheck = CheckRule(new ShipmentMustNotBeTerminalRule(Tracking));
            if (terminalCheck.IsFailure) return terminalCheck;

            var routePoint = RoutePoint.Create(location, description, arrivedAt, type);
            _route.Add(routePoint);

            Tracking = type switch
            {
                RoutePointType.CustomsClearance => Tracking.WithStatus(ShipmentStatus.AtCustoms, $"Package at customs in {location}."),
                RoutePointType.OutForDelivery => Tracking.WithStatus(ShipmentStatus.OutForDelivery, $"Out for delivery in {location}."),
                _ => Tracking.WithStatus(ShipmentStatus.InTransit, $"Arrived at {location}: {description}.")
            };

            Touch();

            AddDomainEvent(new RoutePointAddedEvent(Id, location, type, arrivedAt));

            if (type == RoutePointType.OutForDelivery)
                AddDomainEvent(new ShipmentOutForDeliveryEvent(Id, Tracking.TrackingNumber, location));

            return Result.Success();
        }

        // ─── Delivery ────────────────────────────────────────────

        public Result MarkDelivered(DateTime deliveredAt, string? receivedBy = null)
        {
            var deliveredCheck = CheckRule(new ShipmentMustNotBeDeliveredRule(Tracking.Status));
            if (deliveredCheck.IsFailure) return deliveredCheck;

            var cancelledCheck = CheckRule(new ShipmentMustNotBeCancelledRule(Tracking.Status));
            if (cancelledCheck.IsFailure) return cancelledCheck;

            Tracking = Tracking.WithStatus(ShipmentStatus.Delivered,
                receivedBy is not null
                    ? $"Delivered and received by {receivedBy}."
                    : "Successfully delivered.");

            DeliveredAt = deliveredAt;
            Touch();

            AddDomainEvent(new ShipmentDeliveredEvent(Id, Tracking.TrackingNumber, deliveredAt, receivedBy));
            return Result.Success();
        }

        public Result MarkDeliveryFailed(string reason)
        {
            var cancelledCheck = CheckRule(new ShipmentMustNotBeCancelledRule(Tracking.Status));
            if (cancelledCheck.IsFailure) return cancelledCheck;

            var deliveredCheck = CheckRule(new ShipmentMustNotBeDeliveredRule(Tracking.Status));
            if (deliveredCheck.IsFailure) return deliveredCheck;

            Tracking = Tracking.WithStatus(ShipmentStatus.Failed, $"Delivery failed: {reason}");
            Touch();

            AddDomainEvent(new ShipmentDeliveryFailedEvent(Id, Tracking.TrackingNumber, reason, DateTime.UtcNow));
            return Result.Success();
        }

        // ─── Cancellation ────────────────────────────────────────

        public Result Cancel(string reason, string cancelledBy)
        {
            var cancelledCheck = CheckRule(new ShipmentMustNotBeCancelledRule(Tracking.Status));
            if (cancelledCheck.IsFailure) return cancelledCheck;

            var deliveredCheck = CheckRule(new ShipmentMustNotBeDeliveredRule(Tracking.Status));
            if (deliveredCheck.IsFailure) return deliveredCheck;

            if (string.IsNullOrWhiteSpace(reason))
                return Result.Failure(ShipmentErrors.EmptyCancelReason);

            Tracking = Tracking.WithStatus(ShipmentStatus.Cancelled, $"Cancelled: {reason}");
            CancellationReason = reason;
            CancelledAt = DateTime.UtcNow;
            Touch();

            AddDomainEvent(new ShipmentCancelledEvent(Id, Tracking.TrackingNumber, reason, cancelledBy));
            return Result.Success();
        }

        // ─── Address Update ──────────────────────────────────────

        public Result UpdateDeliveryAddress(DeliveryAddress newAddress)
        {
            var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
            if (editableCheck.IsFailure) return editableCheck;

            var oldAddress = DestinationAddress;
            DestinationAddress = newAddress;
            Touch();

            AddDomainEvent(new DeliveryAddressUpdatedEvent(Id, oldAddress, newAddress));
            return Result.Success();
        }

        // ─── Computed Properties ─────────────────────────────────

        public ShipmentStatus Status => Tracking.Status;
        public int TotalPackages => _packages.Count;
        public decimal TotalWeightKg => _packages.Sum(p => p.Weight.ToKilograms());
        public decimal TotalDeclaredValue => _packages.Sum(p => p.DeclaredValue);
        public bool IsDelivered => Tracking.Status == ShipmentStatus.Delivered;
        public bool IsCancelled => Tracking.Status == ShipmentStatus.Cancelled;
        public bool IsEditable => Tracking.Status is ShipmentStatus.Created or ShipmentStatus.ReadyForPickup;
        public RoutePoint? CurrentLocation => _route.MaxBy(r => r.ArrivedAt);

        // ─── Private Helpers ─────────────────────────────────────

        private void Touch()
        {
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }
}
