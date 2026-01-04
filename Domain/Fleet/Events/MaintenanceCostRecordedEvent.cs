using Domain.Common;

namespace Domain.Fleet.Events
{
    public class MaintenanceCostRecordedEvent : DomainEvent
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