using Domain.Common;

namespace Domain.Shipment.Events
{
    internal class TrackingUpdateAddedEvent : DomainEvent
    {
        private Guid id;
        private string location;

        public TrackingUpdateAddedEvent(Guid id, string location)
        {
            this.id = id;
            this.location = location;
        }
    }
}