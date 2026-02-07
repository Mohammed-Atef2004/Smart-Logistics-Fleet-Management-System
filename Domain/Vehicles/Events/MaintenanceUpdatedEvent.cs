using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal class MaintenanceUpdatedEvent : DomainEvent
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