using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.UpdateName
{
    public record UpdateDriverNameCommand(DriverId Id, string FullName) : IRequest<Unit>;

}