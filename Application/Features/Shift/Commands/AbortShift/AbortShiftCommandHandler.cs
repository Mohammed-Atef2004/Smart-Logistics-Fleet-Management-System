using Domain.Shifts;
using Domain.Shifts.Errors;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.AbortShift;

public sealed class AbortShiftCommandHandler : IRequestHandler<AbortShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftRepository _shiftRepository;

    public AbortShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
    }

    public async Task<Result> Handle(AbortShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(new ShiftId(request.ShiftId), cancellationToken);
        if (shift is null)
            return Result.Failure(ShiftErrors.NotFound(request.ShiftId));

        var result = shift.Abort();
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
