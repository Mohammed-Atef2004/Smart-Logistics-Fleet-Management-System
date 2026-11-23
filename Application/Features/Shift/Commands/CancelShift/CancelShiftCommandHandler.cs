using Domain.Shifts;
using Domain.Shifts.Errors;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.CancelShift;

public sealed class CancelShiftCommandHandler : IRequestHandler<CancelShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftRepository _shiftRepository;

    public CancelShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
    }

    public async Task<Result> Handle(CancelShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(new ShiftId(request.ShiftId), cancellationToken);
        if (shift is null)
            return Result.Failure(ShiftErrors.NotFound(request.ShiftId));

        var result = shift.Cancel();
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
