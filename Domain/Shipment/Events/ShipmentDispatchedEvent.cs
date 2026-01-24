using Domain.Common;

namespace Domain.Shipment.Events
{
    internal class ShipmentDispatchedEvent : DomainEvent
    {
        private Guid id;

        public ShipmentDispatchedEvent(Guid id)
        {
            this.id = id;
        }
    }
}