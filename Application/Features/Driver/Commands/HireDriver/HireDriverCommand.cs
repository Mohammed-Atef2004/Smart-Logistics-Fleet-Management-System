namespace Application.Features.Driver.Commands.HireDriver
{
    public record HireDriverCommand(
        string FullName,
        string LicenseNumber,
        DateTime ExpiryDate,
        string Category
    ) : IRequest<Guid>;
}