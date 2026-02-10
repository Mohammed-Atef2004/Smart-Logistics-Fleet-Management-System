using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal record VehicleUpdatedEvent : DomainEvent
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