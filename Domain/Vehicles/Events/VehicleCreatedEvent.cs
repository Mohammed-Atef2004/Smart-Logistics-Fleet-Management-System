using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal class VehicleCreatedEvent : DomainEvent
    {
        private Guid id;
        private string licensePlate;

        public VehicleCreatedEvent(Guid id, string licensePlate)
        {
            this.id = id;
            this.licensePlate = licensePlate;
        }
    }
}