using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.UpdateLicense;

public sealed class UpdateDriverLicenseCommandHandler : IRequestHandler<UpdateDriverLicenseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverUniquenessChecker _driverUniquenessChecker;

    public UpdateDriverLicenseCommandHandler(
        IUnitOfWork unitOfWork,
        IDriverRepository driverRepository,
        IDriverUniquenessChecker driverUniquenessChecker)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
        _driverUniquenessChecker = driverUniquenessChecker;
    }

    public async Task<Result> Handle(UpdateDriverLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseResult = DriverLicense.Create(request.LicenseNumber, request.ExpiryDate, request.Category);
        if (licenseResult.IsFailure)
            return Result.Failure(licenseResult.Error);

        var driver = await _driverRepository.GetByIdAsync(new DriverId(request.DriverId), cancellationToken);
        if (driver is null)
            return Result.Failure(DriverErrors.NotFound(request.DriverId));

        var result = driver.UpdateLicense(licenseResult.Value, _driverUniquenessChecker);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
