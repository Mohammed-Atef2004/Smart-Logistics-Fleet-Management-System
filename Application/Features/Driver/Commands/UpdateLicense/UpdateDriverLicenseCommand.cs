namespace Application.Features.Driver.Commands.UpdateLicense;

public record UpdateDriverLicenseCommand(
    Guid DriverId,
    string LicenseNumber,
    DateTime ExpiryDate,
    string Category
) : IRequest<Result>;
