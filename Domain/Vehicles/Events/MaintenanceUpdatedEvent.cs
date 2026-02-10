using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal record MaintenanceUpdatedEvent : DomainEvent
    {
        private Guid id;
        private decimal cost;

        public MaintenanceUpdatedEvent(Guid id, decimal cost)
        {
            this.id = id;
            this.cost = cost;
        }
    }
}