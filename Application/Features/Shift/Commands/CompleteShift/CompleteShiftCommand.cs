namespace Application.Features.Shift.Commands.CompleteShift;

/// <summary>Completes an active shift. The id comes from the route.</summary>
public record CompleteShiftCommand(Guid ShiftId) : IRequest<Result>;
