using Application.Features.Fleet.MaintenanceRecord.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Queries.GetById
{
    public class GetMaintenanceRecordByIdQueryHandler:
        IRequestHandler<GetMaintenanceRecordByIdQuery, MaintenanceRecordDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetMaintenanceRecordByIdQueryHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, AutoMapper.IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public Task<MaintenanceRecordDto> Handle(GetMaintenanceRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var maintenanceRecord =  _unitOfWork.MaintenanceRecords.GetByIdAsync(request.Id);
            if (maintenanceRecord.Result == null)
            {
                throw new NullReferenceException("The Id Shouldn't Be Null");
            }
            return Task.FromResult(_mapper.Map<MaintenanceRecordDto>(maintenanceRecord.Result));
        }
    }
}
