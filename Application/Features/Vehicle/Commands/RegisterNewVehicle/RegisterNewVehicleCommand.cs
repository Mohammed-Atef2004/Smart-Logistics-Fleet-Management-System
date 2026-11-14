namespace Application.Features.Vehicle.Commands.RegisterNewVehicle
{
    

    public record RegisterNewVehicleCommand(string PlateNumber, VehicleSpecification Specification)
        : IRequest<Result<VehicleId>>;

}