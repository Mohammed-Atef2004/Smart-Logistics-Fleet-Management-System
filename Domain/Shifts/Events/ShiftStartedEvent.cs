using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events;

/// <summary>Raised when a planned shift actually starts.</summary>
public record ShiftStartedEvent : DomainEvent
{
    public ShiftId ShiftId { get; init; }
    public DriverId DriverId { get; init; }
    public DateTime ShiftStart { get; init; }
    public DateTime ShiftEnd { get; init; }

    public ShiftStartedEvent(ShiftId shiftId, DriverId driverId, DateTime shiftStart, DateTime shiftEnd)
    {
        ShiftId = shiftId;
        DriverId = driverId;
        ShiftStart = shiftStart;
        ShiftEnd = shiftEnd;
    }
}
