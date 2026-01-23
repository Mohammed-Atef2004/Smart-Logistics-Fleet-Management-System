using Application.Features.Fleet.MaintenanceRecord.DTOs;
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

namespace Application.Features.Fleet.Vehicle.Queries.GetAllMaintenanceRecordForVehicle
{
    public class GetAllMaintenanceRecordForVehicleQueryHandler:IRequestHandler<GetAllMaintenanceRecordForVehicleQuery,List<MaintenanceRecordDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

    
        public GetAllMaintenanceRecordForVehicleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MaintenanceRecordDto>> Handle(GetAllMaintenanceRecordForVehicleQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.MaintenanceRecords.EntityQuery
                .ProjectTo<MaintenanceRecordDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);


        }
    }
}
