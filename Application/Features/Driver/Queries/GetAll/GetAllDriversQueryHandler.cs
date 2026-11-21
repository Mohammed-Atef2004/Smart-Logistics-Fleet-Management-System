using Application.Features.Driver.DTOs;
using Domain.Drivers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Driver.Queries.GetAll;

public sealed class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, Result<List<DriverDto>>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public GetAllDriversQueryHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<DriverDto>>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
    {
        // Materialize asynchronously; the old code passed an IQueryable to
        // AutoMapper, which enumerated it synchronously inside the request.
        var drivers = await _driverRepository.EntityQuery
            .AsNoTracking()
            .OrderBy(d => d.FullName)
            .ToListAsync(cancellationToken);

        return Result<List<DriverDto>>.Success(_mapper.Map<List<DriverDto>>(drivers));
    }
}
