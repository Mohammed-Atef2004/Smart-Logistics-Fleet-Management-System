using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events;

/// <summary>Raised when a planned shift is cancelled before it starts.</summary>
public record ShiftCancelledEvent : DomainEvent
{
    public ShiftId ShiftId { get; init; }
    public DriverId DriverId { get; init; }
    public DateTime ShiftStart { get; init; }
    public DateTime ShiftEnd { get; init; }

    public ShiftCancelledEvent(ShiftId shiftId, DriverId driverId, DateTime shiftStart, DateTime shiftEnd)
    {
        ShiftId = shiftId;
        DriverId = driverId;
        ShiftStart = shiftStart;
        ShiftEnd = shiftEnd;
    }
}
