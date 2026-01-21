using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Update
{
    public class UpdateMaintenanceRecordCommandHandler:
        IRequestHandler<UpdateMaintenanceRecordCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMaintenanceRecordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateMaintenanceRecordCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.MaintenanceRecords.GetByIdAsync(request.Id);

            if (result == null)
            {
                throw new Exception($"Maintenance record with ID {request.Id} not found");
            }

            result.Update(
                request.UpdateMaintenanceRecordDto.Type,
                request.UpdateMaintenanceRecordDto.Description,
                request.UpdateMaintenanceRecordDto.Cost,
                request.UpdateMaintenanceRecordDto.MileageAtMaintenance
             ); 

           
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
