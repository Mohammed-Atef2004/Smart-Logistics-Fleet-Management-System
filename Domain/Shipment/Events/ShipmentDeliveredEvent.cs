using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Events
{
    public class ShipmentDeliveredEvent : DomainEvent
    {
        public Guid ShipmentId { get; }

        public ShipmentDeliveredEvent(Guid shipmentId, DateTime value)
        {
            ShipmentId = shipmentId;
        }
    }

}
