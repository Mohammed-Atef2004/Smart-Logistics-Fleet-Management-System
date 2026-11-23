namespace Domain.Shifts.Enums;

/// <summary>
/// Lifecycle of a shift. Only these five states exist, and each transition
/// is enforced by the <c>Shift</c> aggregate:
/// <code>
/// Planned -> Active -> Completed
///    |          |
///    |          +-> Aborted
///    +-> Cancelled
/// </code>
/// Numeric values are explicit and must never be renumbered: the database
/// stores the enum by name, but the numbers are part of the domain contract.
/// </summary>
public enum ShiftStatus
{
    Planned = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    Aborted = 4
}
