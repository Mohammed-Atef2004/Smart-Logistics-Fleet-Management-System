using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts.Events;

/// <summary>
/// Raised when an active shift is completed. Carries the actual window so
/// listeners (payroll, reporting) do not have to re-read the shift.
/// </summary>
public record ShiftCompletedEvent : DomainEvent
{
    public ShiftId ShiftId { get; init; }
    public DriverId DriverId { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime EndedAtUtc { get; init; }

    public ShiftCompletedEvent(
        ShiftId shiftId,
        DriverId driverId,
        DateTime? startedAtUtc,
        DateTime endedAtUtc)
    {
        ShiftId = shiftId;
        DriverId = driverId;
        StartedAtUtc = startedAtUtc;
        EndedAtUtc = endedAtUtc;
    }
}
