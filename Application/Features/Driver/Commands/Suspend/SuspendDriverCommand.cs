using Domain.Drivers.Enums;

namespace Application.Features.Driver.Commands.Suspend;

public record SuspendDriverCommand(Guid DriverId, DriverSuspensionReason Reason) : IRequest<Result>;
