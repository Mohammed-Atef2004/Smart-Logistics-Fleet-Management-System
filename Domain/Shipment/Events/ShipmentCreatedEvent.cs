using Domain.Common;

namespace Domain.Shipment
{
    internal class ShipmentCreatedEvent : DomainEvent
    {
        private string trackingNumber;

        public Guid ShipmentId { get; }

        public ShipmentCreatedEvent(Guid id)
        {
            this.ShipmentId = id;
        }

        public ShipmentCreatedEvent(Guid id, string trackingNumber) : this(id)
        {
            this.trackingNumber = trackingNumber;
        }
    }
}