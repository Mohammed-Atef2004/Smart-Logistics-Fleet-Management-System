using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Events;

public record FuelConsumptionRecordedEvent : DomainEvent
{
    public VehicleId VehicleId { get; init; }
    public decimal Liters { get; init; }
    public decimal OdometerReading { get; init; }

    public FuelConsumptionRecordedEvent(VehicleId vehicleId, decimal liters, decimal odometerReading)
    {
        VehicleId = vehicleId;
        Liters = liters;
        OdometerReading = odometerReading;
    }
}
