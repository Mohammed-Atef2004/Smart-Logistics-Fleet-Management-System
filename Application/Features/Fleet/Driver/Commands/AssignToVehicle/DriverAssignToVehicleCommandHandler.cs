using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.AssignToVehicle
{
    public class DriverAssignToVehicleCommandHandler:IRequestHandler<DriverAssignToVehicleCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DriverAssignToVehicleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DriverAssignToVehicleCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.Id);
            if (driver == null)
            {
                throw new Exception("Driver not found");
            }
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
            if (vehicle == null)
            {
                throw new Exception("Vehicle not found");
            }
            driver.AssignToVehicle(request.VehicleId);
            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }

    }
}
