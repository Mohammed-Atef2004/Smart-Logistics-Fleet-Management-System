using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Delete
{
    public class DeleteMaintenanceRecordCommandHandler:
        IRequestHandler<DeleteMaintenanceRecordCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteMaintenanceRecordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Unit> Handle(DeleteMaintenanceRecordCommand request, CancellationToken cancellationToken)
        {
            var record = await _unitOfWork.MaintenanceRecords.GetByIdAsync(request.Id);

            if (record == null)
            {
                throw new Exception($"Maintenance record with ID {request.Id} not found");
            }

            await _unitOfWork.MaintenanceRecords.DeleteAsync(record);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
