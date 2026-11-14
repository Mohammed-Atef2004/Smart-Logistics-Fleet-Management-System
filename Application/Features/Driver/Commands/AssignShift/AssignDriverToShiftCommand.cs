using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.AssignShift
{
    public record AssignDriverToShiftCommand(DriverId DriverId, DateTime Start, DateTime End) : IRequest<Unit>;

}