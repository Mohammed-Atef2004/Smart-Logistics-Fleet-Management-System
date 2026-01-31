using Application.Features.Shipment.Shipment.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Queries.GetShipmentById
{
    public record GetShipmentByIdQuery(Guid Id): IRequest<ShipmentDto>;

}
