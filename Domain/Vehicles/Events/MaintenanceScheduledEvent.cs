using Domain.SharedKernel;

namespace Domain.Vehicles.Events
{
    public record MaintenanceScheduledEvent : DomainEvent
    {
        public VehicleId id { get; init; }
        public DateTime date { get; init; }

        public MaintenanceScheduledEvent(VehicleId id, DateTime date)
        {
            this.id = id;
            this.date = date;
        }
    }
}