using Domain.SharedKernel;
using Domain.Vehicles.Enums;

namespace Domain.Vehicles.Errors;

public static class VehicleErrors
{
    public static Error NotFound(Guid vehicleId) =>
        new("Vehicle.NotFound", $"Vehicle with ID '{vehicleId}' was not found.");

    public static Error NotAvailable =>
        new("Vehicle.NotAvailable", "Vehicle is not available");

    public static Error PlateAlreadyExists =>
        new("Vehicle.PlateAlreadyExists", "Plate number already exists");

    public static Error InvalidPlateNumber =>
        new("Vehicle.InvalidPlateNumber", "Invalid plate number");

    public static Error InvalidFuelConsumption =>
        new("Vehicle.InvalidFuelConsumption", "Invalid fuel consumption value");

    public static Error VehicleAlreadyRetired =>
        new("Vehicle.AlreadyRetired", "Vehicle is already retired");

    public static Error InvalidStatusTransition(VehicleStatus from, VehicleStatus to) =>
        new("Vehicle.InvalidStatusTransition",
            $"Illegal status transition: a vehicle cannot go from '{from}' to '{to}'.");
}
