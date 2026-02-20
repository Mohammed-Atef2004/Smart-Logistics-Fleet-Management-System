using Domain.SharedKernel;

namespace Domain.Vehicles.ValueObjects;

public record FuelConsumption
{
    public decimal Liters { get; init; }
    public decimal OdometerReading { get; init; }

    private FuelConsumption() { } // For EF Core
    private FuelConsumption(decimal liters, decimal odometerReading)
    {
        Liters = liters;
        OdometerReading = odometerReading;
    }

    public static Result<FuelConsumption> Create(decimal liters, decimal odometerReading)
    {
        if (liters <= 0)
            return Result<FuelConsumption>.Failure(new Error("Fuel.InvalidLiters", "Liters must be greater than zero."));

        if (odometerReading <= 0)
            return Result<FuelConsumption>.Failure(new Error("Fuel.InvalidOdometer", "Odometer reading must be positive."));

        return Result<FuelConsumption>.Success(new FuelConsumption(liters, odometerReading));
    }
}