namespace Application.Features.Shipment.Commands.Cancel;

/// <summary>Cancels a shipment that has not been delivered yet.</summary>
public sealed record CancelShipmentCommand(
    Guid ShipmentId,
    string Reason,
    string CancelledBy) : IRequest<Result>;
