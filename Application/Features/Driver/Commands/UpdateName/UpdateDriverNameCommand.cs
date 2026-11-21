namespace Application.Features.Driver.Commands.UpdateName;

public record UpdateDriverNameCommand(Guid DriverId, string FullName) : IRequest<Result>;
