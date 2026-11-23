using Domain.Shifts;
using Domain.Shifts.Errors;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Commands.StartShift;

public sealed class StartShiftCommandHandler : IRequestHandler<StartShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftRepository _shiftRepository;

    public StartShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
    }

    public async Task<Result> Handle(StartShiftCommand request, CancellationToken cancellationToken)
    {
        // Tracked lookup: the old code read through the AsNoTracking query
        // root and then called Update on a detached graph.
        var shift = await _shiftRepository.GetByIdAsync(new ShiftId(request.ShiftId), cancellationToken);
        if (shift is null)
            return Result.Failure(ShiftErrors.NotFound(request.ShiftId));

        var result = shift.Start();
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
