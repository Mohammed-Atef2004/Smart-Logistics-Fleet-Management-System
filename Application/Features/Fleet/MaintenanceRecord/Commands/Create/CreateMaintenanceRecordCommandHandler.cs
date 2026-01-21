using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Create
{
    public class CreateMaintenanceRecordCommandHandler:IRequestHandler<CreateMaintenanceRecordCommand,Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateMaintenanceRecordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Guid> Handle(CreateMaintenanceRecordCommand request, CancellationToken cancellationToken)
        {
            var maintenanceRecord = _mapper.Map<Domain.Fleet.Entities.MaintenanceRecord>(request.MaintenanceRecordDto);
            await _unitOfWork.MaintenanceRecords.AddAsync(maintenanceRecord);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return maintenanceRecord.Id;
        }
    }
}
