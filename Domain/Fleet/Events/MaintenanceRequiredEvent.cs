using Domain.Common;
using Domain.Fleet.Enums;

namespace Domain.Fleet.Events
{
    // 1. Vehicle Maintenance Trigger
    public class MaintenanceRequiredEvent : DomainEvent
    {
        public Guid VehicleId { get; }
        public int CurrentMileage { get; }

        public MaintenanceRequiredEvent(Guid vehicleId, int currentMileage)
        {
            VehicleId = vehicleId;
            CurrentMileage = currentMileage;
        }
    }

}