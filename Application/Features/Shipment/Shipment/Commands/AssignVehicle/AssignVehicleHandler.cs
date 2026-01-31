using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.AssignVehicle
{
    public class AssignVehicleHandler : IRequestHandler<AssignVehicleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignVehicleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }
        public async Task<Unit> Handle(AssignVehicleCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.Id);
            
            if (shipment == null)
            {
                throw new Exception("Shipment not found");
            }
            shipment.AssignVehicle(request.VehicleId);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
