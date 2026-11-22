using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.Suspend;

public sealed class SuspendDriverCommandHandler : IRequestHandler<SuspendDriverCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;

    public SuspendDriverCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
    }

    public async Task<Result> Handle(SuspendDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(new DriverId(request.DriverId), cancellationToken);
        if (driver is null)
            return Result.Failure(DriverErrors.NotFound(request.DriverId));

        // The old handler ignored this Result, so suspending an already
        // suspended or terminated driver reported success.
        var result = driver.Suspend(request.Reason);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
