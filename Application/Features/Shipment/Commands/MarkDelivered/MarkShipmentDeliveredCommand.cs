namespace Application.Features.Shipment.Commands.MarkDelivered;

/// <summary>Marks a shipment in transit as delivered.</summary>
public sealed record MarkShipmentDeliveredCommand(
    Guid ShipmentId,
    DateTime DeliveredAt,
    string? ReceivedBy = null) : IRequest<Result>;
