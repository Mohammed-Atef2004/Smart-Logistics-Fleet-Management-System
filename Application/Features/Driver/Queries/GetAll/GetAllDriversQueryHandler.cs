using Domain.Drivers;
using Application.Features.Driver.DTOs;

namespace Application.Features.Driver.Queries.GetAll
{
    public sealed class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, List<DriverDto>>
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        public GetAllDriversQueryHandler(IDriverRepository driverRepository, IMapper mapper)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
        }
        public async Task<List<DriverDto>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            var drivers = _driverRepository.EntityQuery.Select(x => x);
            return _mapper.Map<List<DriverDto>>(drivers);

        }
    }
}