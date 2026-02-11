using Domain.Common;

namespace Domain.Vehicles.Events
{
    internal record MaintenanceUpdatedEvent : DomainEvent
    {
        public Guid id { get; init; }
        public decimal cost { get; init; }

        public MaintenanceUpdatedEvent(Guid id, decimal cost)
        {
            this.id = id;
            this.cost = cost;
        }
    }
}