using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.Enums;
using Domain.Shifts.Errors;
using Domain.Shifts.Events;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts;

/// <summary>
/// Shift aggregate root. A shift is a time window assigned to one driver and
/// moves through a strict lifecycle:
/// <code>
/// Planned -> Active -> Completed
///    |          |
///    |          +-> Aborted
///    +-> Cancelled
/// </code>
/// Every transition is validated here; the old implementation accepted any
/// transition (e.g. cancelling a cancelled shift, or "completing" a shift by
/// cancelling it) and never recorded when the shift actually ran.
/// </summary>
public sealed class Shift : AggregateRoot<ShiftId>
{
    /// <summary>Hard ceiling for a single shift. Guards against typos such as a 300-hour shift.</summary>
    public const int MaxDurationHours = 24;

    public DriverId DriverId { get; private set; }
    public DateTime ShiftStart { get; private set; }
    public DateTime ShiftEnd { get; private set; }
    public ShiftStatus Status { get; private set; }

    /// <summary>When the driver actually started; null until <see cref="Start"/>.</summary>
    public DateTime? StartedAtUtc { get; private set; }

    /// <summary>When the shift actually ended (completed or aborted); null while open.</summary>
    public DateTime? EndedAtUtc { get; private set; }

    public TimeSpan Duration => ShiftEnd - ShiftStart;

    public bool IsActive => Status == ShiftStatus.Active;

    private Shift() { } // EF Core

    private Shift(ShiftId id, DriverId driverId, DateTime start, DateTime end) : base(id)
    {
        DriverId = driverId;
        ShiftStart = start;
        ShiftEnd = end;
        Status = ShiftStatus.Planned;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a planned shift. The driver id must be set and the window must
    /// be ordered and no longer than <see cref="MaxDurationHours"/>; both
    /// bounds are normalized to UTC so comparisons never depend on
    /// <see cref="DateTime.Kind"/>.
    /// </summary>
    public static Result<Shift> Create(DriverId driverId, DateTime start, DateTime end)
    {
        if (driverId.IsEmpty)
            return Result<Shift>.Failure(ShiftErrors.InvalidDriver);

        start = NormalizeToUtc(start);
        end = NormalizeToUtc(end);

        var window = ValidateWindow(start, end);
        if (window.IsFailure)
            return Result<Shift>.Failure(window.Error);

        var shift = new Shift(ShiftId.New(), driverId, start, end);
        shift.AddDomainEvent(new ShiftCreatedEvent(shift.Id, driverId, start, end));

        return Result<Shift>.Success(shift);
    }

    /// <summary>Starts a planned shift whose window has not already elapsed.</summary>
    public Result Start()
    {
        if (Status != ShiftStatus.Planned)
            return Result.Failure(ShiftErrors.CannotStart);

        var now = DateTime.UtcNow;
        if (now >= ShiftEnd)
            return Result.Failure(ShiftErrors.AlreadyEnded);

        Status = ShiftStatus.Active;
        StartedAtUtc = now;
        UpdatedAt = now;

        AddDomainEvent(new ShiftStartedEvent(Id, DriverId, ShiftStart, ShiftEnd));
        return Result.Success();
    }

    /// <summary>Completes an active shift and records the actual end time.</summary>
    public Result Complete()
    {
        if (Status != ShiftStatus.Active)
            return Result.Failure(ShiftErrors.CannotComplete);

        var now = DateTime.UtcNow;
        Status = ShiftStatus.Completed;
        EndedAtUtc = now;
        UpdatedAt = now;

        AddDomainEvent(new ShiftCompletedEvent(Id, DriverId, StartedAtUtc, now));
        return Result.Success();
    }

    /// <summary>
    /// Cancels a shift that has not started yet. Cancelling an active shift
    /// is an <see cref="Abort"/>, and cancelling twice is rejected instead of
    /// silently raising a second event.
    /// </summary>
    public Result Cancel()
    {
        if (Status != ShiftStatus.Planned)
            return Result.Failure(ShiftErrors.CannotCancel);

        Status = ShiftStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ShiftCancelledEvent(Id, DriverId, ShiftStart, ShiftEnd));
        return Result.Success();
    }

    /// <summary>Aborts an active shift that will not be completed.</summary>
    public Result Abort()
    {
        if (Status != ShiftStatus.Active)
            return Result.Failure(ShiftErrors.CannotAbort);

        var now = DateTime.UtcNow;
        Status = ShiftStatus.Aborted;
        EndedAtUtc = now;
        UpdatedAt = now;

        AddDomainEvent(new ShiftAbortedEvent(Id, DriverId, now));
        return Result.Success();
    }

    /// <summary>
    /// Moves a planned shift to a new window. Rescheduling a shift that is
    /// already running or finished is rejected; moving it to the exact same
    /// window is a no-op so callers can retry safely.
    /// </summary>
    public Result Reschedule(DateTime start, DateTime end)
    {
        if (Status != ShiftStatus.Planned)
            return Result.Failure(ShiftErrors.CannotReschedule);

        start = NormalizeToUtc(start);
        end = NormalizeToUtc(end);

        var window = ValidateWindow(start, end);
        if (window.IsFailure)
            return Result.Failure(window.Error);

        if (start == ShiftStart && end == ShiftEnd)
            return Result.Success();

        ShiftStart = start;
        ShiftEnd = end;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ShiftRescheduledEvent(Id, DriverId, start, end));
        return Result.Success();
    }

    /// <summary>Half-open overlap test used for in-memory scheduling checks.</summary>
    public bool Overlaps(DateTime start, DateTime end) =>
        ShiftStart < end && start < ShiftEnd;

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private static Result ValidateWindow(DateTime start, DateTime end)
    {
        if (start >= end)
            return Result.Failure(ShiftErrors.InvalidDuration);

        if (end - start > TimeSpan.FromHours(MaxDurationHours))
            return Result.Failure(ShiftErrors.DurationTooLong);

        return Result.Success();
    }

    /// <summary>
    /// Treats an unspecified <see cref="DateTime.Kind"/> as UTC (the whole
    /// system works in UTC) and converts local values, so stored windows are
    /// always comparable. Public so callers can normalize before running
    /// cross-aggregate checks against the same values the aggregate will store.
    /// </summary>
    public static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
