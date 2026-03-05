using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.AddRoutePoint
{
    public record AddRoutePointCommand(
        ShipmentId Id,
        string location, 
        string description,
        DateTime arrivedAt,
        RoutePointType type
    ) : IRequest<Result>;
}
