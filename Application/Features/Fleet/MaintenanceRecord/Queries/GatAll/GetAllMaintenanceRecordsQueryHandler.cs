using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Application.Features.Fleet.MaintenanceRecord.Queries.GetAll;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Queries.GatAll
{
    public class GetAllMaintenanceRecordsQueryHandler:IRequestHandler<GetAllMaintenanceRecordsQuery,List<MaintenanceRecordDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllMaintenanceRecordsQueryHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<MaintenanceRecordDto>> Handle(GetAllMaintenanceRecordsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.MaintenanceRecords.EntityQuery;

            if (request.VehicleId.HasValue)
            {
                query = query.Where(x => x.VehicleId == request.VehicleId.Value);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(x => x.Type == request.Type.Value);
            }

            return await query
                .OrderByDescending(x => x.MaintenanceDate) 
                .ProjectTo<MaintenanceRecordDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
