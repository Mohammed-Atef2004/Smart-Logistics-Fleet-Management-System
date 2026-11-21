using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Drivers.Events;

/// <summary>Raised when a shift is assigned to a driver.</summary>
public record DriverShiftAssignedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public ShiftId ShiftId { get; init; }

    public DriverShiftAssignedEvent(DriverId driverId, ShiftId shiftId)
    {
        DriverId = driverId;
        ShiftId = shiftId;
    }
}
