using Domain.Interfaces.Repositories;
using Domain.Vehicles.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.AddMaintenanceRecord
{
    public class AddMaintenanceRecordHandler : IRequestHandler<AddMaintenanceRecordCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddMaintenanceRecordHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(AddMaintenanceRecordCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.maintenanceRecordDto.VehicleId);
            if (vehicle == null) throw new Exception("Vehicle not found");

            vehicle.AddMaintenanceRecord(
                (MaintenanceType)request.maintenanceRecordDto.Type,
                request.maintenanceRecordDto.Description,
                request.maintenanceRecordDto.Cost);

            await _unitOfWork.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
