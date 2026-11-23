namespace Application.Features.Shift.Commands.StartShift;

/// <summary>Starts a planned shift. The id comes from the route.</summary>
public record StartShiftCommand(Guid ShiftId) : IRequest<Result>;
