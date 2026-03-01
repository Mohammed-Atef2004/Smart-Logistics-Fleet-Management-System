using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Events
{
    public sealed record RoutePointAddedEvent(
    ShipmentId ShipmentId,
    string Location,
    RoutePointType PointType,
    DateTime ArrivedAt) : DomainEvent;
}
