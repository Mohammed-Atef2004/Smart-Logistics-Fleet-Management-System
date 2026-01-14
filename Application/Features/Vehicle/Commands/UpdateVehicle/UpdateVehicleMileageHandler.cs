using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.UpdateVehicle
{
    public class UpdateVehicleMileageHandler : IRequestHandler<UpdateVehicleMileageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVehicleMileageHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateVehicleMileageCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
            if (vehicle == null) throw new Exception("Vehicle not found"); 

            vehicle.UpdateMileage(request.NewMileage);

            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
