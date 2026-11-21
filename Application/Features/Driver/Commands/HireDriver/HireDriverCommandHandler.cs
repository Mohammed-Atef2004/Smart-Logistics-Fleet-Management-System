using Domain.Drivers;
using Domain.Drivers.ValueObjects;
using DriverAggregate = Domain.Drivers.Driver;

namespace Application.Features.Driver.Commands.HireDriver;

public sealed class HireDriverCommandHandler : IRequestHandler<HireDriverCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverUniquenessChecker _driverUniquenessChecker;
    private readonly ILogger<HireDriverCommandHandler> _logger;

    public HireDriverCommandHandler(
        IUnitOfWork unitOfWork,
        IDriverRepository driverRepository,
        IDriverUniquenessChecker driverUniquenessChecker,
        ILogger<HireDriverCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
        _driverUniquenessChecker = driverUniquenessChecker;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(HireDriverCommand request, CancellationToken cancellationToken)
    {
        // Check every value-object Result before touching .Value — the
        // old handler threw a raw Exception and bypassed the Result contract.
        var licenseResult = DriverLicense.Create(request.LicenseNumber, request.ExpiryDate, request.Category);
        if (licenseResult.IsFailure)
            return Result<Guid>.Failure(licenseResult.Error);

        var driverResult = DriverAggregate.Hire(request.FullName, licenseResult.Value, _driverUniquenessChecker);
        if (driverResult.IsFailure)
        {
            _logger.LogWarning("Hiring failed for {FullName}: {Error}", request.FullName, driverResult.Error);
            return Result<Guid>.Failure(driverResult.Error);
        }

        await _driverRepository.AddAsync(driverResult.Value);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Driver hired with id {DriverId}", driverResult.Value.Id.Value);
        return Result<Guid>.Success(driverResult.Value.Id.Value);
    }
}
