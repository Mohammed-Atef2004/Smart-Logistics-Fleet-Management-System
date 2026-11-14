namespace Application.Features.Vehicle.Commands.RetireVehicle
{
    public record RetireVehicleCommand(Guid VehicleId) : IRequest<Result>;
}