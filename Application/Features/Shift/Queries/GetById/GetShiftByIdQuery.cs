using Application.Features.Shift.DTOs;

namespace Application.Features.Shift.Queries.GetById;

public record GetShiftByIdQuery(Guid ShiftId) : IRequest<Result<ShiftDto>>;
