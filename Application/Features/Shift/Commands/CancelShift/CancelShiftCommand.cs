namespace Application.Features.Shift.Commands.CancelShift;

/// <summary>Cancels a shift that has not started yet.</summary>
public record CancelShiftCommand(Guid ShiftId) : IRequest<Result>;
