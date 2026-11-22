namespace Domain.Drivers.ValueObjects;

/// <summary>
/// Strongly-typed identifier for the Driver aggregate.
/// A record struct (like <c>VehicleId</c>) keeps the id allocation-free
/// and gives value equality for free.
/// </summary>
public readonly record struct DriverId(Guid Value)
{
    public static DriverId New() => new(Guid.NewGuid());

    public bool IsEmpty => Value == Guid.Empty;
}
