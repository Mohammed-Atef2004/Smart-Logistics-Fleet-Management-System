using MediatR;

namespace Application.Features.Vehicles.Commands.CreateVehicle;

public record CreateVehicleCommand : IRequest<Guid>
{
    public string Make { get; init; } = default!;
    public string Model { get; init; } = default!;
    public string Vin { get; init; } = default!;
    public int Year { get; init; }
    public string LicensePlate { get; init; } = default!;
}