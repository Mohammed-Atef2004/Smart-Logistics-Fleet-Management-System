using Domain.Common;

namespace Domain.Shipment.Events
{
    internal class PackageAddedToShipmentEvent : DomainEvent
    {
        private Guid id1;
        private Guid id2;

        public PackageAddedToShipmentEvent(Guid id1, Guid id2)
        {
            this.id1 = id1;
            this.id2 = id2;
        }
    }
}