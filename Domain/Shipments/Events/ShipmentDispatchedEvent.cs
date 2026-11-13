using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Events
{
    public sealed record ShipmentDispatchedEvent(
     ShipmentId ShipmentId,
     string TrackingNumber,
     string CarrierName) : DomainEvent;
}
