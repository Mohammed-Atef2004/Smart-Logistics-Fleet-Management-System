using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.MarkDeliveryFailed
{
    public record MarkShipmentDeliveryFailedCommand
    (
        ShipmentId Id,
        string reason
    ): IRequest<Result>;
}