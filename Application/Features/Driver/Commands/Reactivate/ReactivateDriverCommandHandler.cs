using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.Reactivate;

public sealed class ReactivateDriverCommandHandler : IRequestHandler<ReactivateDriverCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;

    public ReactivateDriverCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
    }

    public async Task<Result> Handle(ReactivateDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(new DriverId(request.DriverId), cancellationToken);
        if (driver is null)
            return Result.Failure(DriverErrors.NotFound(request.DriverId));

        var result = driver.Reactivate();
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
