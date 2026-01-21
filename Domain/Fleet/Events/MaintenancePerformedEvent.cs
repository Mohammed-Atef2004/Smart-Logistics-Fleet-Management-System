using Domain.Common;
using Domain.Fleet.Enums;

namespace Domain.Fleet.Events
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