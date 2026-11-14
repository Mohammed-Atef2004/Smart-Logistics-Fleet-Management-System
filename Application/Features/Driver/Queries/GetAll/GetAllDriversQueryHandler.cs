using Application.Common.Interfaces;
using Application.Features.Driver.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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