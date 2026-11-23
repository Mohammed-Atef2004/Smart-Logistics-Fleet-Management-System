using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events;

/// <summary>
/// Raised when an active shift is aborted (interrupted) instead of being
/// completed normally. Uses the previously unreachable
/// <c>ShiftStatus.Aborted</c> state.
/// </summary>
public record ShiftAbortedEvent : DomainEvent
{
    public ShiftId ShiftId { get; init; }
    public DriverId DriverId { get; init; }
    public DateTime EndedAtUtc { get; init; }

    public ShiftAbortedEvent(ShiftId shiftId, DriverId driverId, DateTime endedAtUtc)
    {
        ShiftId = shiftId;
        DriverId = driverId;
        EndedAtUtc = endedAtUtc;
    }
}
