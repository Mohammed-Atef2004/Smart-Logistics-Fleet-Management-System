namespace Application.Features.Driver.Commands.Reactivate;

public record ReactivateDriverCommand(Guid DriverId) : IRequest<Result>;
