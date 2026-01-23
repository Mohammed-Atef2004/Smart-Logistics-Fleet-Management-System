using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.Delete
{
    public class DeleteVehicleCommandHandler: IRequestHandler<DeleteVehicleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
       
        public DeleteVehicleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(DeleteVehicleCommand requestId, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(requestId.Id);
            if (vehicle == null) throw new Exception("Vehicle not found");
            _unitOfWork.Vehicles.Delete(vehicle);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
