using Domain.SharedKernel;
using Domain.Vehicles.Enums;

namespace Domain.Vehicles.Events
{
    internal record MaintenancePerformedEvent : DomainEvent
    {
        public Guid vehicleId { get; init; }
        public MaintenanceType type { get; init; }
        public decimal cost { get; init; }

        public MaintenancePerformedEvent(Guid vehicleId, MaintenanceType type, decimal cost)
        {
            this.vehicleId = vehicleId;
            this.type = type;
            this.cost = cost;
        }
    }
}