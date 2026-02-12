using Domain.SharedKernel;

namespace Domain.Vehicles.Events
{
    public record FuelConsumptionRecordedEvent : DomainEvent
    {
        public VehicleId id { get; init; }
        public decimal liters { get; init; }
        public decimal odometerReading { get; init; }

        public FuelConsumptionRecordedEvent(VehicleId id, decimal liters, decimal odometerReading)
        {
            this.id = id;
            this.liters = liters;
            this.odometerReading = odometerReading;
        }
    }
}