using Domain.Shifts;
using Domain.Shifts.Errors;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.CompleteShift;

public sealed class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftRepository _shiftRepository;

    public CompleteShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
    }

    public async Task<Result> Handle(CompleteShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(new ShiftId(request.ShiftId), cancellationToken);
        if (shift is null)
            return Result.Failure(ShiftErrors.NotFound(request.ShiftId));

        // Regression: the old handler called Cancel() and threw raw
        // exceptions, so "complete" never completed anything.
        var result = shift.Complete();
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
