namespace Application.Features.Driver.Commands.ClearShift;

public record ClearDriverShiftCommand(Guid DriverId) : IRequest<Result>;
