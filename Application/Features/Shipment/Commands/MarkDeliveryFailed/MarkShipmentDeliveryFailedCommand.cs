namespace Application.Features.Shipment.Commands.MarkDeliveryFailed;

/// <summary>Records a failed delivery attempt for a shipment in transit.</summary>
public sealed record MarkShipmentDeliveryFailedCommand(
    Guid ShipmentId,
    string Reason) : IRequest<Result>;
