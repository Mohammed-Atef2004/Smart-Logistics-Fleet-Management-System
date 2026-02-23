using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverShiftClearedEvent : DomainEvent
{
    private DriverId Id;

    public DriverShiftClearedEvent(DriverId id)
    {
        Id= id;
    }
}