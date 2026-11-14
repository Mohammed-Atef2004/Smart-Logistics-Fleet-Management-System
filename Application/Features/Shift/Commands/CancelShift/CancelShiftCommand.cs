using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.CancelShift
{
    public record CancelShiftCommand(ShiftId ShiftId) : IRequest<Result>;
}