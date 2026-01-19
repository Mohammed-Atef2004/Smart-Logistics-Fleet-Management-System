using MediatR;

namespace Application.Features.Vehicle.Commands.CreateVehicle;

public record CreateVehicleCommand(string LicensePlate, string Model, int CurrentMileage) : IRequest<Guid>;