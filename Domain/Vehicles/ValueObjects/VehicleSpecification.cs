using Domain.SharedKernel;

namespace Domain.Vehicles.ValueObjects;

/// <summary>
/// Make/Year/EngineType trio. Private constructor + Create factory like
/// every other value object. NOTE: this type must never be bound
/// directly from HTTP JSON (private constructor) — commands receive
/// primitives and build it through <see cref="Create"/>.
/// </summary>
public sealed record VehicleSpecification
{
    public string Model { get; private set; } = default!;
    public int Year { get; private set; }
    public string EngineType { get; private set; } = default!;

    private VehicleSpecification(string model, int year, string engineType)
    {
        Model = model;
        Year = year;
        EngineType = engineType;
    }

    public static Result<VehicleSpecification> Create(string model, int year, string engineType)
    {
        if (string.IsNullOrWhiteSpace(model))
            return Result<VehicleSpecification>.Failure(new Error("VehicleSpecification.ModelRequired", "Model cannot be empty."));
        if (year < 1886 || year > DateTime.UtcNow.Year + 1) // First car invented in 1886
            return Result<VehicleSpecification>.Failure(new Error("VehicleSpecification.YearOutOfRange", "Year is out of valid range."));
        if (string.IsNullOrWhiteSpace(engineType))
            return Result<VehicleSpecification>.Failure(new Error("VehicleSpecification.EngineTypeRequired", "Engine type cannot be empty."));

        return Result<VehicleSpecification>.Success(new VehicleSpecification(model, year, engineType));
    }
}
