using Domain.Common;

namespace Domain.Fleet.Events
{
    internal class VehicleLicensePlateUpdatedEvent : DomainEvent
    {
        private Guid id;
        private string licensePlate;

        public VehicleLicensePlateUpdatedEvent(Guid id, string licensePlate)
        {
            this.id = id;
            this.licensePlate = licensePlate;
        }
    }
}