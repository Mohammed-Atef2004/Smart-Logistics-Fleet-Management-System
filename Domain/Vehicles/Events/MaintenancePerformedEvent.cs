using Domain.Common;
using Domain.Vehicles.Enums;

namespace Domain.Vehicles.Events
{
    internal class MaintenancePerformedEvent : DomainEvent
    {
        private Guid vehicleId;
        private MaintenanceType type;
        private decimal cost;

        public MaintenancePerformedEvent(Guid vehicleId, MaintenanceType type, decimal cost)
        {
            this.vehicleId = vehicleId;
            this.type = type;
            this.cost = cost;
        }
    }
}