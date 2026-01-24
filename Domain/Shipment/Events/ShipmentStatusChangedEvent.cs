using Domain.Common;
using Domain.Shipment.Enums;

namespace Domain.Shipment.Events
{
    internal class ShipmentStatusChangedEvent : DomainEvent
    {
        private Guid id;
        private ShipmentStatus status;

        public ShipmentStatusChangedEvent(Guid id, ShipmentStatus status)
        {
            this.id = id;
            this.status = status;
        }
    }
}