using Domain.Common;

namespace Domain.Vehicles.Events
{
    public record MaintenanceCostRecordedEvent : DomainEvent
    {
        public Guid VehicleId { get; }
        public decimal Amount { get; }

        public MaintenanceCostRecordedEvent(Guid vehicleId, decimal amount)
        {
            VehicleId = vehicleId;
            Amount = amount;
        }
    }
}