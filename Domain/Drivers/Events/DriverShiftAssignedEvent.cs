using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverShiftAssignedEvent : DomainEvent
{
    public DriverId Id;
    public ShiftId ShiftId;

    public DriverShiftAssignedEvent(DriverId id, ShiftId shiftId)
    {
        Id = id;
        ShiftId = shiftId;
    }
}