using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.UpdateMileage
{
    public class UpdateMileageCommandHandler : IRequestHandler<UpdateMileageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateMileageCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateMileageCommand request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
            if (vehicle == null) throw new Exception("Vehicle not found");

            vehicle.UpdateMileage(request.NewMileage);

            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
