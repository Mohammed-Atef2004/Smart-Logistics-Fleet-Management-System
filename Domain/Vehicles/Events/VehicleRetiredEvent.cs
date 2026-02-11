using Domain.Common;

namespace Domain.Vehicles.Events
{
    public record VehicleRetiredEvent : DomainEvent
    {
        public VehicleId id { get; init; }

        public VehicleRetiredEvent(VehicleId id)
        {
            this.id = id;
        }
    }
}