using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.CompleteShift
{
    public record CompleteShiftCommand(ShiftId ShiftId) : IRequest<Result>;
}