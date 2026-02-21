using Domain.SharedKernel;

public sealed record VehicleSpecification
{
    private VehicleSpecification(string model, int year, string engineType)
    {
        Model = model;
        Year = year;
        EngineType = engineType;
    }

    public string Model { get; private set; }
    public int Year { get; private set; }
    public  string EngineType { get; private set; }
    public static Result<VehicleSpecification> Create(string model, int year, string engineType)
    {
        if (string.IsNullOrWhiteSpace(model))
            return Result<VehicleSpecification>.Failure(new Error("ModelError","Model cannot be empty."));
        if (year < 1886 || year > DateTime.UtcNow.Year + 1) // First car invented in 1886
            return Result<VehicleSpecification>.Failure(new Error("ModelError", "Year is out of valid range."));
        if (string.IsNullOrWhiteSpace(engineType))
            return Result<VehicleSpecification>.Failure(new Error("ModelError", "Engine type cannot be empty."));
        var spec = new VehicleSpecification(model, year, engineType);
        return Result<VehicleSpecification>.Success(spec);
    }


}
