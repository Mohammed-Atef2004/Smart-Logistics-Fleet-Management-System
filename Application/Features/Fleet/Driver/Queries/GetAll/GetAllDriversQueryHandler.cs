using Application.Features.Fleet.Driver.Dtos;
using Application.Features.Fleet.Driver.Queries.GetAll;
using Application.Features.Fleet.Drivers.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Fleet.Drivers.Queries.GetAll
{
    public class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, List<DriverDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllDriversQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DriverDto>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Drivers.EntityQuery;

            if (request.IsActive.HasValue)
            {
                query = query.Where(d => d.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(d => d.FullName.Contains(term)
                                      || d.LicenseNumber.Contains(term));
            }

            return await query
                .ProjectTo<DriverDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}