using Application.Features.Shipment.Shipment.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.Create
{
    public record CreateShipmentCommand(ShipmentDto shipmentDto): IRequest<Guid>;

}
