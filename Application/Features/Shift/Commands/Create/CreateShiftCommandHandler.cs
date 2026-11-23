using Application.Features.Driver.DomainServices;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Shift.Commands.Create;

/// <summary>
/// Single creation path. The old handler built a shift directly, so a shift
/// could be created for a suspended or nonexistent driver, could overlap an
/// existing shift, and never set the driver's <c>CurrentShiftId</c> (only the
/// driver endpoint did that). Delegating to <see cref="IDriverShiftService"/>
/// keeps both endpoints consistent.
/// </summary>
public sealed class CreateShiftCommandHandler : IRequestHandler<CreateShiftCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverShiftService _driverShiftService;

    public CreateShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IDriverShiftService driverShiftService)
    {
        _unitOfWork = unitOfWork;
        _driverShiftService = driverShiftService;
    }

    public async Task<Result<Guid>> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
    {
        var result = await _driverShiftService.AssignDriverToShift(
            new DriverId(request.DriverId), request.Start, request.End);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result<Guid>.Success(result.Value.Id.Value);
    }
}
