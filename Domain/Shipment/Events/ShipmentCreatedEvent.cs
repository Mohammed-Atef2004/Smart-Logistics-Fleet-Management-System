using Domain.Common;

namespace Domain.Shipment
{
    internal class ShipmentCreatedEvent : DomainEvent
    {
        public Guid ShipmentId { get; }

        public ShipmentCreatedEvent(Guid id)
        {
            this.ShipmentId = id;
        }
    }
}