using Application.Features.Fleet.Vehicle.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.GetAll
{
    public class GetAllVehiclesQueryHandler: IRequestHandler<GetAllVehiclesQuery, List<VehicleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllVehiclesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<VehicleDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Vehicles.EntityQuery;

            if (request.Status.HasValue)
            {
                query = query.Where(v => v.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Model))
            {
                query = query.Where(v => v.Model.Contains(request.Model));
            }

            return await query
                .ProjectTo<VehicleDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
