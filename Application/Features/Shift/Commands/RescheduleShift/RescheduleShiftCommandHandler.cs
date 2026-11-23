using Domain.Shifts;
using Domain.Shifts.Enums;
using Domain.Shifts.Errors;
using Domain.Shifts.ValueObjects;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Features.Shift.Commands.RescheduleShift;

public sealed class RescheduleShiftCommandHandler : IRequestHandler<RescheduleShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftRepository _shiftRepository;

    public RescheduleShiftCommandHandler(IUnitOfWork unitOfWork, IShiftRepository shiftRepository)
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
    }

    public async Task<Result> Handle(RescheduleShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(new ShiftId(request.ShiftId), cancellationToken);
        if (shift is null)
            return Result.Failure(ShiftErrors.NotFound(request.ShiftId));

        // Only a planned shift can move. Checked before the overlap query so
        // an invalid state never triggers a pointless database round-trip.
        if (shift.Status != ShiftStatus.Planned)
            return Result.Failure(ShiftErrors.CannotReschedule);

        var start = ShiftAggregate.NormalizeToUtc(request.Start);
        var end = ShiftAggregate.NormalizeToUtc(request.End);

        // The shift itself is excluded so keeping the same window is not
        // reported as a conflict with itself.
        var overlaps = await _shiftRepository.HasOverlappingShift(
            shift.DriverId, start, end, shift.Id);
        if (overlaps)
            return Result.Failure(ShiftErrors.Overlap);

        var result = shift.Reschedule(start, end);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
