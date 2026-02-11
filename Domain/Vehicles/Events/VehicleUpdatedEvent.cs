using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal record VehicleUpdatedEvent : DomainEvent
    {
        public Guid id { get; init; }
        public string licensePlate { get; init; }

        public VehicleUpdatedEvent(Guid id, string licensePlate)
        {
            this.id = id;
            this.licensePlate = licensePlate;
        }
    }
}