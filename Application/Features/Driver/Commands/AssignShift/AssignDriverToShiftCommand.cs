namespace Application.Features.Driver.Commands.AssignShift;

/// <summary>
/// Creates a shift for a driver and marks it as the driver's current
/// shift. Primitives only, so the request binds from JSON; the handler
/// builds the domain ids.
/// </summary>
public record AssignDriverToShiftCommand(Guid DriverId, DateTime Start, DateTime End) : IRequest<Result>;
