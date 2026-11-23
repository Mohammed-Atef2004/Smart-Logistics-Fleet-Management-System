namespace Domain.Shifts.ValueObjects;

/// <summary>
/// Strongly-typed identifier for the Shift aggregate.
/// A record struct (like <c>DriverId</c> and <c>VehicleId</c>) is
/// allocation-free, gives value equality for free, and never throws on
/// materialization - an empty id is reported through <see cref="IsEmpty"/>
/// and rejected by the aggregate that consumes it.
/// </summary>
public readonly record struct ShiftId(Guid Value)
{
    public static ShiftId New() => new(Guid.NewGuid());

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
