using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverShiftStartedEvent : DomainEvent
{
    public DriverId Id;
    public ShiftId ShiftId;

    public DriverShiftStartedEvent(DriverId id, ShiftId shiftId)
    {
        Id = id;
        ShiftId = shiftId;
    }
}