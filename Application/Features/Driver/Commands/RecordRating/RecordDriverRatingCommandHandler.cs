using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.RecordRating;

public sealed class RecordDriverRatingCommandHandler : IRequestHandler<RecordDriverRatingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;

    public RecordDriverRatingCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
    }

    public async Task<Result> Handle(RecordDriverRatingCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(new DriverId(request.DriverId), cancellationToken);
        if (driver is null)
            return Result.Failure(DriverErrors.NotFound(request.DriverId));

        var result = driver.RecordTripRating(request.Rating);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
