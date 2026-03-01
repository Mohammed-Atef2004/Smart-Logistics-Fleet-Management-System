using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Events
{
    public sealed record ShipmentDeliveryFailedEvent(
    ShipmentId ShipmentId,
    string TrackingNumber,
    string FailureReason,
    DateTime FailedAt) : DomainEvent;
}
