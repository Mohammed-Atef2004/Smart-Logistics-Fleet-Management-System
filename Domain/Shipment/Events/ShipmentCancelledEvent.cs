using Domain.Common;

namespace Domain.Shipment.Events
{
    internal class ShipmentCancelledEvent : DomainEvent
    {
        private Guid id;
        private string reason;

        public ShipmentCancelledEvent(Guid id, string reason)
        {
            this.id = id;
            this.reason = reason;
        }
    }
}