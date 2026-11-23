using Domain.SharedKernel;

namespace Domain.Shifts.Errors;

/// <summary>
/// Single source of truth for shift errors. Handlers and the aggregate
/// must use these instead of building ad-hoc <see cref="Error"/> instances,
/// otherwise the same failure ends up with different codes per endpoint.
/// </summary>
public static class ShiftErrors
{
    public static Error NotFound(Guid shiftId) =>
        new("Shift.NotFound", $"Shift with ID '{shiftId}' was not found.");

    public static readonly Error InvalidDriver =
        new("Shift.InvalidDriver", "A valid driver id is required.");

    public static readonly Error InvalidDuration =
        new("Shift.InvalidDuration", "Shift start must be before shift end.");

    public static readonly Error DurationTooLong =
        new("Shift.DurationTooLong", "A shift cannot be longer than 24 hours.");

    public static readonly Error AlreadyEnded =
        new("Shift.AlreadyEnded", "The shift window has already ended.");

    public static readonly Error CannotStart =
        new("Shift.CannotStart", "Only a planned shift can be started.");

    public static readonly Error CannotComplete =
        new("Shift.CannotComplete", "Only an active shift can be completed.");

    public static readonly Error CannotCancel =
        new("Shift.CannotCancel", "Only a planned shift can be cancelled.");

    public static readonly Error CannotAbort =
        new("Shift.CannotAbort", "Only an active shift can be aborted.");

    public static readonly Error CannotReschedule =
        new("Shift.CannotReschedule", "Only a planned shift can be rescheduled.");

    public static readonly Error Overlap =
        new("Shift.Overlap", "The driver already has a shift in this time window.");
}
