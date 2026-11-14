using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.UpdateLicence
{
    public record UpdateDriverLicenseCommand(
    DriverId DriverId,
    string LicenseNumber,
    DateTime ExpiryDate,
    string Category
) : IRequest<Unit>;
}