using Domain.SharedKernel;

namespace Domain.Shipments.Errors;

/// <summary>
/// Single source of truth for shipment errors. Every rule and handler returns
/// one of these, so a failure keeps the same code no matter which endpoint
/// produced it.
/// </summary>
public static class ShipmentErrors
{
    public static Error NotFound(Guid shipmentId) => new(
        "Shipment.NotFound",
        $"Shipment with ID '{shipmentId}' was not found.");

    public static Error DuplicateTrackingNumber(string trackingNumber) => new(
        "Shipment.DuplicateTrackingNumber",
        $"A shipment with tracking number '{trackingNumber}' already exists.");

    public static readonly Error EmptySenderId = new(
        "Shipment.EmptySenderId",
        "Sender ID cannot be empty.");

    public static readonly Error EmptyTrackingNumber = new(
        "Shipment.EmptyTrackingNumber",
        "Tracking number cannot be empty.");

    public static readonly Error EmptyCancelReason = new(
        "Shipment.EmptyCancelReason",
        "Cancellation reason cannot be empty.");

    public static readonly Error EmptyCancelledBy = new(
        "Shipment.EmptyCancelledBy",
        "The user cancelling the shipment is required.");

    public static readonly Error EmptyCarrier = new(
        "Shipment.EmptyCarrier",
        "A carrier name is required.");

    public static readonly Error EmptyFailureReason = new(
        "Shipment.EmptyFailureReason",
        "A delivery failure reason is required.");

    public static readonly Error InvalidAddress = new(
        "Shipment.InvalidAddress",
        "A valid delivery address is required.");

    public static readonly Error InvalidDeliveredAt = new(
        "Shipment.InvalidDeliveredAt",
        "The delivery timestamp is required.");

    public static readonly Error PackageNotFound = new(
        "Shipment.PackageNotFound",
        "The specified package was not found in the shipment.");

    public static readonly Error NoPackages = new(
        "Shipment.NoPackages",
        "A shipment must contain at least one package before it can be dispatched.");

    public static readonly Error NotEditable = new(
        "Shipment.NotEditable",
        "The shipment can no longer be modified in its current status.");

    public static readonly Error AlreadyCancelled = new(
        "Shipment.AlreadyCancelled",
        "The shipment has already been cancelled.");

    public static readonly Error AlreadyDelivered = new(
        "Shipment.AlreadyDelivered",
        "The shipment has already been delivered.");

    public static readonly Error InvalidStatusTransition = new(
        "Shipment.InvalidStatusTransition",
        "The shipment cannot move to the requested status from its current status.");
}
