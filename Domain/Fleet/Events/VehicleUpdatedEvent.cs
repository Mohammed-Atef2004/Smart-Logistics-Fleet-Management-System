using Domain.Common;

namespace Domain.Fleet.Events
{
    internal class VehicleUpdatedEvent : DomainEvent
    {
        private Guid id;
        private string licensePlate;

        public VehicleUpdatedEvent(Guid id, string licensePlate)
        {
            this.id = id;
            this.licensePlate = licensePlate;
        }
    }
}