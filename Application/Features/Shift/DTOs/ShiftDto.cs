namespace Application.Features.Shift.DTOs;

/// <summary>
/// Flat read model for a shift. Ids are plain <see cref="Guid"/>s so the
/// payload is stable for clients and does not leak domain value objects.
/// </summary>
public record ShiftDto(
    Guid Id,
    Guid DriverId,
    DateTime Start,
    DateTime End,
    string Status,
    DateTime? StartedAtUtc,
    DateTime? EndedAtUtc);
