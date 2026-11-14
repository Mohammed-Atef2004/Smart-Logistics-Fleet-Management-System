using Application.Features.Driver.DTOs;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Queries.GetById
{
    public record GetDriverByIdQuery(DriverId Id): IRequest<DriverDto>;

}