using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Drivers.Events;

/// <summary>
/// Raised when a driver's current shift is cleared. The cleared shift id
/// travels in the payload (the old event kept it in a private field, so
/// no handler could ever read it).
/// </summary>
public record DriverShiftClearedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public ShiftId ShiftId { get; init; }

    public DriverShiftClearedEvent(DriverId driverId, ShiftId shiftId)
    {
        DriverId = driverId;
        ShiftId = shiftId;
    }
}
