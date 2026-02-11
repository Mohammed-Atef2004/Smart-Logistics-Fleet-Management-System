using Domain.Common;
using Domain.Vehicles.Errors;

public sealed record FuelConsumption
{
    public decimal LitersPer100Km { get; }

    private FuelConsumption(decimal value)
    {
        LitersPer100Km = value;
    }
    private FuelConsumption() { } // EF Core

    public static Result<FuelConsumption> Create(decimal value)
    {
        if (value <= 0 || value > 100)
            return Result<FuelConsumption>.Failure(VehicleErrors.InvalidFuelConsumption);

        return Result<FuelConsumption>.Success(new FuelConsumption(value));
    }
}
