namespace Domain.Shipments.ValueObjects;

/// <summary>
/// Strongly-typed identifier for the Shipment aggregate. A record struct
/// (like <c>DriverId</c>, <c>VehicleId</c> and <c>ShiftId</c>) is
/// allocation-free, gives value equality for free, and never throws while
/// materializing from the database - an empty id is reported through
/// <see cref="IsEmpty"/> instead.
/// </summary>
public readonly record struct ShipmentId(Guid Value)
{
    public static ShipmentId New() => new(Guid.NewGuid());

    public static ShipmentId From(Guid value) => new(value);

    public static ShipmentId From(string value) => new(Guid.Parse(value));

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
