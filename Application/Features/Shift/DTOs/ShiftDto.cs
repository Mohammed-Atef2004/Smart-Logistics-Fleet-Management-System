using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.DTOs
{
    public record ShiftDto(ShiftId Id, Guid DriverId, DateTime Start, DateTime End, string Status);
}