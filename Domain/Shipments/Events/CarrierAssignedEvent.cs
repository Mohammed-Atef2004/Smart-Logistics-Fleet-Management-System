using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.Events
{
    public sealed record CarrierAssignedEvent(
    ShipmentId ShipmentId,
    string CarrierName,
    string? EstimatedDeliveryDate) : DomainEvent;
}
