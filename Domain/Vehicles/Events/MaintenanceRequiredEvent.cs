using Domain.Common;
using Domain.Vehicles.Enums;

namespace Domain.Vehicles.Events
{
    // 1. Vehicle Maintenance Trigger
    public record MaintenanceRequiredEvent : DomainEvent
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