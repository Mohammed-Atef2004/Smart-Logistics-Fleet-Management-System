using Application.Features.Driver.DTOs;
using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Queries.GetById;

public sealed class GetDriverByIdQueryHandler : IRequestHandler<GetDriverByIdQuery, Result<DriverDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public GetDriverByIdQueryHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<Result<DriverDto>> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(new DriverId(request.Id), cancellationToken);

        // A missing driver is a Result failure, not a raw Exception — the
        // old handler threw and the middleware turned it into a 500.
        if (driver is null)
            return Result<DriverDto>.Failure(DriverErrors.NotFound(request.Id));

        return Result<DriverDto>.Success(_mapper.Map<DriverDto>(driver));
    }
}
