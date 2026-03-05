using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.UpdateDeliveryAddress
{
    public record UpdateShipmentDeliveryAddressCommand
    (
        ShipmentId Id,
        DeliveryAddress newAddress
    ): IRequest<Result>;

}
