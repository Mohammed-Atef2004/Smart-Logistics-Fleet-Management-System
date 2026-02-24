using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

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