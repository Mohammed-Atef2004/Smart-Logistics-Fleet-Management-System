namespace Domain.Shipments.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a package inside a shipment. It is a record
/// struct for the same reasons as <see cref="ShipmentId"/>: no allocation and
/// no throwing constructor.
/// </summary>
public readonly record struct PackageId(Guid Value)
{
    public static PackageId New() => new(Guid.NewGuid());

    public static PackageId From(Guid value) => new(value);

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
