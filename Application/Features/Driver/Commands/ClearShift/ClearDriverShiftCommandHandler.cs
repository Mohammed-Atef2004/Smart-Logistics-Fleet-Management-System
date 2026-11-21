using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.ClearShift;

/// <summary>
/// Releases the driver's current shift. The aggregate method existed but
/// had no use case or endpoint, so a driver could never be freed.
/// </summary>
public sealed class ClearDriverShiftCommandHandler : IRequestHandler<ClearDriverShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;

    public ClearDriverShiftCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
    }

    public async Task<Result> Handle(ClearDriverShiftCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(new DriverId(request.DriverId), cancellationToken);
        if (driver is null)
            return Result.Failure(DriverErrors.NotFound(request.DriverId));

        var result = driver.ClearShift();
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
