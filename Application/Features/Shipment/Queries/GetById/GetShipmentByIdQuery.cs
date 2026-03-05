using Application.Features.Shipment.DTOs;
using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Queries.GetById
{
    public record GetShipmentByIdQuery
    (
     ShipmentId Id
    ): IRequest<Result<ShipmentDetailsDto>>;
}
