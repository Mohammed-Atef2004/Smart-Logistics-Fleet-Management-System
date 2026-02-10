using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal record MaintenanceScheduledEvent : DomainEvent
    {
        private VehicleId id;
        private DateTime date;

        public MaintenanceScheduledEvent(VehicleId id, DateTime date)
        {
            this.id = id;
            this.date = date;
        }
    }
}