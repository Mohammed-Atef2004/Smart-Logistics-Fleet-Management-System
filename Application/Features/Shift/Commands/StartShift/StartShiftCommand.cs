using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.StartShift
{
    public record StartShiftCommand(ShiftId ShiftId) : IRequest<Result>;
}