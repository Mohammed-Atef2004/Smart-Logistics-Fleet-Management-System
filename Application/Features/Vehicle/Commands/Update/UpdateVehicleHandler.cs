using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.UpdateVehicle
{
    public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVehicleHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
            if (vehicle == null) throw new Exception("Vehicle not found"); 

            vehicle.Update(vehicle.LicensePlate,request.UpdateVechicleDto.Model,request.UpdateVechicleDto.CurrentMileage);
            _unitOfWork.Vehicles.Update(vehicle);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
