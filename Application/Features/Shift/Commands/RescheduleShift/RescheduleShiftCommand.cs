namespace Application.Features.Shift.Commands.RescheduleShift;

/// <summary>Moves a planned shift to a new time window.</summary>
public record RescheduleShiftCommand(Guid ShiftId, DateTime Start, DateTime End) : IRequest<Result>;
