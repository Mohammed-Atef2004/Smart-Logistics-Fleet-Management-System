namespace Application.Features.Driver.Commands.Terminate;

public record TerminateDriverCommand(Guid DriverId) : IRequest<Result>;
