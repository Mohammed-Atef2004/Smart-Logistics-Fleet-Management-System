using Domain.SharedKernel;
using Domain.Shipments.Enums;
using Domain.Shipments.Errors;
using Domain.Shipments.Events;
using Domain.Shipments.Rules;
using Domain.Shipments.ValueObjects;

namespace Domain.Shipments;

/// <summary>
/// Shipment aggregate root. It owns its packages and route points and guards
/// a small lifecycle:
/// <code>
/// Created -> ReadyForPickup -> InTransit -> AtCustoms -> OutForDelivery -> Delivered
///    |              |             |            |              |
///    +--------------+-------------+------------+--------------+-> Cancelled
///                                             (failed delivery) -> Failed
/// </code>
/// Every state change validates the current status first and propagates the
/// value-object failures instead of swallowing them.
/// </summary>
public sealed class Shipment : AggregateRoot<ShipmentId>
{
    public const int MaxSenderIdLength = 100;

    public string SenderId { get; private set; } = default!;
    public string? RecipientName { get; private set; }
    public string? RecipientPhone { get; private set; }
    public DeliveryAddress DestinationAddress { get; private set; } = default!;
    public TrackingInfo Tracking { get; private set; } = default!;
    public ShipmentPriority Priority { get; private set; }

    /// <summary>Optimistic concurrency token, incremented on every change.</summary>
    public int Version { get; private set; }

    private readonly List<Package> _packages = [];
    public IReadOnlyList<Package> Packages => _packages.AsReadOnly();

    private readonly List<RoutePoint> _route = [];
    public IReadOnlyList<RoutePoint> Route => _route.AsReadOnly();

    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? SpecialInstructions { get; private set; }

    private Shipment() { } // EF Core

    private Shipment(ShipmentId id) : base(id) { }

    // ─── Factory ─────────────────────────────────────────────────

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

        if (destinationAddress is null)
            return Result<Shipment>.Failure(ShipmentErrors.InvalidAddress);

        var trackingResult = TrackingInfo.Create(
            trackingNumber,
            ShipmentStatus.Created,
            "Shipment created and awaiting packages.");
        if (trackingResult.IsFailure)
            return Result<Shipment>.Failure(trackingResult.Error);

        var shipment = new Shipment(ShipmentId.New())
        {
            SenderId = senderId.Trim(),
            DestinationAddress = destinationAddress,
            Priority = priority,
            RecipientName = NormalizeOptional(recipientName),
            RecipientPhone = NormalizeOptional(recipientPhone),
            SpecialInstructions = NormalizeOptional(specialInstructions),
            CreatedAt = DateTime.UtcNow,
            Tracking = trackingResult.Value
        };

        shipment.AddDomainEvent(new ShipmentCreatedEvent(
            shipment.Id,
            shipment.SenderId,
            shipment.DestinationAddress,
            shipment.Tracking.TrackingNumber));

        return Result<Shipment>.Success(shipment);
    }

    // ─── Package management ──────────────────────────────────────

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
        var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
        if (editableCheck.IsFailure)
            return Result<PackageId>.Failure(editableCheck.Error);

        if (weight is null)
            return Result<PackageId>.Failure(PackageErrors.InvalidWeight);
        if (dimensions is null)
            return Result<PackageId>.Failure(PackageErrors.InvalidDimensions);

        var packageResult = Package.Create(
            description,
            weight,
            dimensions,
            contentCategory,
            isFragile,
            requiresRefrigeration,
            declaredValue,
            currency);
        if (packageResult.IsFailure)
            return Result<PackageId>.Failure(packageResult.Error);

        var package = packageResult.Value;
        _packages.Add(package);
        Touch();

        AddDomainEvent(new PackageAddedToShipmentEvent(
            Id, package.Id, package.Description, package.Weight.ToKilograms()));

        return Result<PackageId>.Success(package.Id);
    }

    public Result RemovePackage(PackageId packageId)
    {
        var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
        if (editableCheck.IsFailure)
            return editableCheck;

        var package = _packages.FirstOrDefault(p => p.Id == packageId);
        if (package is null)
            return Result.Failure(PackageErrors.NotFound(packageId.Value));

        _packages.Remove(package);
        Touch();

        AddDomainEvent(new PackageRemovedFromShipmentEvent(Id, packageId));
        return Result.Success();
    }

    // ─── Carrier & dispatch ──────────────────────────────────────

    public Result AssignCarrier(string carrierName, DateTime? estimatedDeliveryDate = null)
    {
        var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
        if (editableCheck.IsFailure)
            return editableCheck;

        if (string.IsNullOrWhiteSpace(carrierName))
            return Result.Failure(ShipmentErrors.EmptyCarrier);

        Tracking = Tracking.WithCarrier(carrierName.Trim(), estimatedDeliveryDate);
        Touch();

        AddDomainEvent(new CarrierAssignedEvent(Id, carrierName.Trim(), estimatedDeliveryDate));
        return Result.Success();
    }

    public Result Dispatch()
    {
        var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
        if (editableCheck.IsFailure)
            return editableCheck;

        var packagesCheck = CheckRule(new ShipmentMustHavePackagesRule(_packages.Count));
        if (packagesCheck.IsFailure)
            return packagesCheck;

        var carrierCheck = CheckRule(new ShipmentMustHaveCarrierRule(Tracking.CarrierName));
        if (carrierCheck.IsFailure)
            return carrierCheck;

        Tracking = Tracking.WithStatus(ShipmentStatus.InTransit, "Shipment dispatched and in transit.");
        Touch();

        AddDomainEvent(new ShipmentDispatchedEvent(Id, Tracking.TrackingNumber, Tracking.CarrierName!));
        return Result.Success();
    }

    // ─── Route management ────────────────────────────────────────

    public Result AddRoutePoint(string location, string description, DateTime arrivedAt, RoutePointType type)
    {
        if (Tracking.Status == ShipmentStatus.Cancelled)
            return Result.Failure(ShipmentErrors.AlreadyCancelled);

        var terminalCheck = CheckRule(new ShipmentMustNotBeTerminalRule(Tracking));
        if (terminalCheck.IsFailure)
            return terminalCheck;

        var routePointResult = RoutePoint.Create(location, description, arrivedAt, type);
        if (routePointResult.IsFailure)
            return routePointResult;

        var routePoint = routePointResult.Value;
        _route.Add(routePoint);

        Tracking = type switch
        {
            RoutePointType.CustomsClearance =>
                Tracking.WithStatus(ShipmentStatus.AtCustoms, $"Package at customs in {routePoint.Location}."),
            RoutePointType.OutForDelivery =>
                Tracking.WithStatus(ShipmentStatus.OutForDelivery, $"Out for delivery in {routePoint.Location}."),
            _ => Tracking.WithStatus(ShipmentStatus.InTransit, $"Arrived at {routePoint.Location}: {routePoint.Description}.")
        };

        Touch();

        AddDomainEvent(new RoutePointAddedEvent(Id, routePoint.Location, type, routePoint.ArrivedAt));

        if (type == RoutePointType.OutForDelivery)
            AddDomainEvent(new ShipmentOutForDeliveryEvent(Id, Tracking.TrackingNumber, routePoint.Location));

        return Result.Success();
    }

    // ─── Delivery ────────────────────────────────────────────────

    public Result MarkDelivered(DateTime deliveredAt, string? receivedBy = null)
    {
        var stateCheck = EnsureDeliveryInProgress();
        if (stateCheck.IsFailure)
            return stateCheck;

        if (deliveredAt == default)
            return Result.Failure(ShipmentErrors.InvalidDeliveredAt);

        var deliveredAtUtc = NormalizeToUtc(deliveredAt);

        Tracking = Tracking.WithStatus(
            ShipmentStatus.Delivered,
            string.IsNullOrWhiteSpace(receivedBy)
                ? "Successfully delivered."
                : $"Delivered and received by {receivedBy.Trim()}.");

        DeliveredAt = deliveredAtUtc;
        Touch();

        AddDomainEvent(new ShipmentDeliveredEvent(
            Id,
            Tracking.TrackingNumber,
            deliveredAtUtc,
            string.IsNullOrWhiteSpace(receivedBy) ? null : receivedBy.Trim()));

        return Result.Success();
    }

    public Result MarkDeliveryFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(ShipmentErrors.EmptyFailureReason);

        var stateCheck = EnsureDeliveryInProgress();
        if (stateCheck.IsFailure)
            return stateCheck;

        var failedAt = DateTime.UtcNow;
        Tracking = Tracking.WithStatus(ShipmentStatus.Failed, $"Delivery failed: {reason.Trim()}");
        Touch();

        AddDomainEvent(new ShipmentDeliveryFailedEvent(
            Id, Tracking.TrackingNumber, reason.Trim(), failedAt));

        return Result.Success();
    }

    // ─── Cancellation ────────────────────────────────────────────

    public Result Cancel(string reason, string cancelledBy)
    {
        if (Tracking.Status == ShipmentStatus.Cancelled)
            return Result.Failure(ShipmentErrors.AlreadyCancelled);

        if (Tracking.Status == ShipmentStatus.Delivered)
            return Result.Failure(ShipmentErrors.AlreadyDelivered);

        if (Tracking.Status == ShipmentStatus.Returned)
            return Result.Failure(ShipmentErrors.InvalidStatusTransition);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(ShipmentErrors.EmptyCancelReason);

        if (string.IsNullOrWhiteSpace(cancelledBy))
            return Result.Failure(ShipmentErrors.EmptyCancelledBy);

        Tracking = Tracking.WithStatus(ShipmentStatus.Cancelled, $"Cancelled: {reason.Trim()}");
        CancellationReason = reason.Trim();
        CancelledAt = DateTime.UtcNow;
        Touch();

        AddDomainEvent(new ShipmentCancelledEvent(
            Id, Tracking.TrackingNumber, CancellationReason, cancelledBy.Trim()));

        return Result.Success();
    }

    // ─── Address update ──────────────────────────────────────────

    public Result UpdateDeliveryAddress(DeliveryAddress newAddress)
    {
        var editableCheck = CheckRule(new ShipmentMustBeEditableRule(Tracking.Status));
        if (editableCheck.IsFailure)
            return editableCheck;

        if (newAddress is null)
            return Result.Failure(ShipmentErrors.InvalidAddress);

        var oldAddress = DestinationAddress;
        DestinationAddress = newAddress;
        Touch();

        AddDomainEvent(new DeliveryAddressUpdatedEvent(Id, oldAddress, newAddress));
        return Result.Success();
    }

    // ─── Computed properties ─────────────────────────────────────

    public ShipmentStatus Status => Tracking.Status;
    public int TotalPackages => _packages.Count;
    public decimal TotalWeightKg => _packages.Sum(p => p.Weight.ToKilograms());
    public decimal TotalDeclaredValue => _packages.Sum(p => p.DeclaredValue);
    public bool IsDelivered => Tracking.Status == ShipmentStatus.Delivered;
    public bool IsCancelled => Tracking.Status == ShipmentStatus.Cancelled;
    public bool IsEditable => Tracking.Status is ShipmentStatus.Created or ShipmentStatus.ReadyForPickup;
    public RoutePoint? CurrentLocation => _route.MaxBy(r => r.ArrivedAt);

    // ─── Helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Delivered and failed can only be reached while the shipment is on its
    /// way. Marking a freshly created shipment as delivered used to succeed.
    /// </summary>
    private Result EnsureDeliveryInProgress()
    {
        if (Tracking.Status == ShipmentStatus.Cancelled)
            return Result.Failure(ShipmentErrors.AlreadyCancelled);

        if (Tracking.Status == ShipmentStatus.Delivered)
            return Result.Failure(ShipmentErrors.AlreadyDelivered);

        if (Tracking.Status is ShipmentStatus.InTransit
            or ShipmentStatus.AtCustoms
            or ShipmentStatus.OutForDelivery)
            return Result.Success();

        return Result.Failure(ShipmentErrors.InvalidStatusTransition);
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
