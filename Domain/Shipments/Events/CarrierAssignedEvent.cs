using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
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
