using Application.Features.Shift.DTOs;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.Queries.GetById
{
    public record GetShiftByIdQuery(ShiftId shiftId): IRequest<Result<ShiftDto>>;
    
}