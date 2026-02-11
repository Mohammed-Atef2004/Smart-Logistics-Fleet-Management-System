using Domain.Common;
using Domain.Vehicles.Enums;

namespace Domain.Vehicles.Events
{
    // 1. Vehicle Maintenance Trigger
    public record MaintenanceRequiredEvent : DomainEvent
    {
        public Guid VehicleId { get; init; }
        public int CurrentMileage { get; init; }

        public MaintenanceRequiredEvent(Guid vehicleId, int currentMileage)
        {
            VehicleId = vehicleId;
            CurrentMileage = currentMileage;
        }
    }

}