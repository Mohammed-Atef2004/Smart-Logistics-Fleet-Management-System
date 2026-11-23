using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events;

/// <summary>Raised when a planned shift is moved to a new time window.</summary>
public record ShiftRescheduledEvent : DomainEvent
{
    public ShiftId ShiftId { get; init; }
    public DriverId DriverId { get; init; }
    public DateTime ShiftStart { get; init; }
    public DateTime ShiftEnd { get; init; }

    public ShiftRescheduledEvent(ShiftId shiftId, DriverId driverId, DateTime shiftStart, DateTime shiftEnd)
    {
        ShiftId = shiftId;
        DriverId = driverId;
        ShiftStart = shiftStart;
        ShiftEnd = shiftEnd;
    }
}
