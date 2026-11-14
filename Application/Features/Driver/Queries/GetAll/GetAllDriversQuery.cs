using Application.Features.Driver.DTOs;

namespace Application.Features.Driver.Queries.GetAll
{
    public record GetAllDriversQuery: IRequest<List<DriverDto>>;
}