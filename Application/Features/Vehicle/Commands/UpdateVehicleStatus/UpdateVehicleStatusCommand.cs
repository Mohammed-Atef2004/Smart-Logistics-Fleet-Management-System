using Domain.Vehicles.Enums;

namespace Application.Features.Vehicle.Commands.UpdateVehicleStatus
{
    public record UpdateVehicleStatusCommand(
    Guid VehicleId,
    VehicleStatus NewStatus) : IRequest<Result>;
}