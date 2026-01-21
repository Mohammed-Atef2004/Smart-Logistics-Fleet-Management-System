using Application.Features.Fleet.MaintenanceRecord.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
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
            var maintenanceRecords = await _unitOfWork.MaintenanceRecords.GetAllAsync();
            return _mapper.Map<List<MaintenanceRecordDto>>(maintenanceRecords);
        }
    }
}
