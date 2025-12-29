using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Events
{


    public class ShipmentStatusUpdatedEvent : DomainEvent
    {
        public Guid ShipmentId { get; }
        public ShipmentStatus Status { get; }

        public ShipmentStatusUpdatedEvent(Guid shipmentId, ShipmentStatus status)
        {
            ShipmentId = shipmentId;
            Status = status;
        }
    }

}
