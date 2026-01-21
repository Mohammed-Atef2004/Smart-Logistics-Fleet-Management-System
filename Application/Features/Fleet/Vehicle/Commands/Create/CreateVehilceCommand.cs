using MediatR;

namespace Application.Features.Fleet.Vehicle.Commands.Create;

public record CreateVehicleCommand(string LicensePlate, string Model, int CurrentMileage) : IRequest<Guid>;