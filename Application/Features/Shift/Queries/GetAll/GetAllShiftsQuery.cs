using Application.Features.Shift.DTOs;

namespace Application.Features.Shift.Queries.GetAll
{
    public record GetAllShiftsQuery: IRequest<Result<List<ShiftDto>>>;

}