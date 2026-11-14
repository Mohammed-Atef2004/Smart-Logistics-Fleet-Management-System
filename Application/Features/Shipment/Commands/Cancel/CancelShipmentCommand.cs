using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.Cancel
{
    public record CancelShipmentCommand
    (
        ShipmentId Id,
        string reason,
        string cancelledBy
    ): IRequest<Result>;
}