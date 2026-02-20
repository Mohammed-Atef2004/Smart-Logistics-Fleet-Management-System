using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverPerformanceDroppedEvent : DomainEvent
{
    public DriverId Id;
    public double Value;

    public DriverPerformanceDroppedEvent(DriverId id, double value)
    {
        Id = id;
        Value = value;
    }
}