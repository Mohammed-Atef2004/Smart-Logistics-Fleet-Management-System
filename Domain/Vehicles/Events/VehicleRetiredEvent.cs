using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal record VehicleRetiredEvent : DomainEvent
    {
        private VehicleId id;

        public VehicleRetiredEvent(VehicleId id)
        {
            this.id = id;
        }
    }
}