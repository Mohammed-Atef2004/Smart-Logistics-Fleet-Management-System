using Application.Features.Fleet.MaintenanceRecord.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
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
            var vehicles = await _unitOfWork.Vehicles.GetAllMaintenanceRecordsForVehicle(request.Id);
            if (vehicles == null)
            {
                throw new Exception("No Records Found for this Vehicle found");
            }
            if (vehicles.MaintenanceRecords.Count == 0)
            {
                throw new Exception("No Records Found for this Vehicle found");
            }
           
            var result=_mapper.Map<List<MaintenanceRecordDto>>(vehicles.MaintenanceRecords);
            return result;
        }
    }
}
