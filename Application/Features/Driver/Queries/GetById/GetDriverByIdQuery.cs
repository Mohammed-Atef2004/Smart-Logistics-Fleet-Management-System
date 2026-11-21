using Application.Features.Driver.DTOs;

namespace Application.Features.Driver.Queries.GetById;

public record GetDriverByIdQuery(Guid Id) : IRequest<Result<DriverDto>>;
