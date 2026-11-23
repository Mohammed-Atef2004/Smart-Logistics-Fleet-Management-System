namespace Application.Features.Shift.Commands.Create;

/// <summary>
/// Schedules a shift for a driver. Primitives only so the request binds
/// from JSON; the handler builds the domain ids. The driver must exist, be
/// active, and have no overlapping shift - that coordination lives in
/// <c>IDriverShiftService</c>.
/// </summary>
public record CreateShiftCommand(Guid DriverId, DateTime Start, DateTime End) : IRequest<Result<Guid>>;
