namespace Application.Features.Shift.Commands.AbortShift;

/// <summary>Aborts an active shift that will not be completed.</summary>
public record AbortShiftCommand(Guid ShiftId) : IRequest<Result>;
