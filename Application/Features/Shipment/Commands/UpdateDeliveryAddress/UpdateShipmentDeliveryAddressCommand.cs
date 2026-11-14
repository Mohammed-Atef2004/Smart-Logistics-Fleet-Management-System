using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.UpdateDeliveryAddress
{
    public record UpdateShipmentDeliveryAddressCommand
    (
        ShipmentId Id,
        DeliveryAddress newAddress
    ): IRequest<Result>;

}