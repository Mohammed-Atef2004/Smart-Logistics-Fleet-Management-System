using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.MarkDelivered
{
    public record MarkShipmentDeliveredCommand
    (
        ShipmentId Id,
        DateTime deliveredAt, 
        string? receivedBy = null
    ): IRequest<Result>;
}