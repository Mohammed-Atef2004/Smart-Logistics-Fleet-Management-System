using Domain.Drivers.ValueObjects;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.Create
{
    public record CreateShiftCommand(DriverId driverId, DateTime Start, DateTime End) : IRequest<Result<ShiftId>>;
}