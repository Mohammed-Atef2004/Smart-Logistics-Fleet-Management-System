using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverShiftEndedEvent : DomainEvent
{
    public DriverId Id;

    public DriverShiftEndedEvent(DriverId id)
    {
        Id = id;
    }
}