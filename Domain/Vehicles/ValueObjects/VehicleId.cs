namespace Domain.Vehicles.ValueObjects;

/// <summary>Strongly-typed identifier for the Vehicle aggregate.</summary>
public readonly record struct VehicleId(Guid Value)
{
    public static VehicleId New() => new(Guid.NewGuid());
}
