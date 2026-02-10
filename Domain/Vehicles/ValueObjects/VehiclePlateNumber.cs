using Domain.Common;
using Domain.Vehicles.Errors;
public sealed record VehiclePlateNumber
{
    public string Value { get; }

    private VehiclePlateNumber(string value)
    {
        Value = value;
    }

    public static Result<VehiclePlateNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<VehiclePlateNumber>.Failure(VehicleErrors.InvalidPlateNumber);

        return Result<VehiclePlateNumber>.Success(new VehiclePlateNumber(value));
    }
}
