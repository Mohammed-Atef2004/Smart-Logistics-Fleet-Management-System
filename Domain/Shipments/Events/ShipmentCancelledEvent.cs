using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Events
{
    public sealed record ShipmentCancelledEvent(
    ShipmentId ShipmentId,
    string TrackingNumber,
    string Reason,
    string CancelledBy) : DomainEvent;
}
