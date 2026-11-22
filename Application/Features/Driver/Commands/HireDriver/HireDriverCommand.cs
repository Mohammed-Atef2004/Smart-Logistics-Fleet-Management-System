namespace Application.Features.Driver.Commands.HireDriver;

/// <summary>
/// Transport DTO: primitives only. A domain value object with a private
/// constructor can never be bound from JSON, so the handler builds the
/// <c>DriverLicense</c> and converts its failure into a Result.
/// </summary>
public record HireDriverCommand(
    string FullName,
    string LicenseNumber,
    DateTime ExpiryDate,
    string Category
) : IRequest<Result<Guid>>;
