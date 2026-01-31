using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.CancelShipment
{
    public class CancelShipmentHandler: IRequestHandler<CancelShipmentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancelShipmentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(CancelShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.Id);
            
            if (shipment == null)
            {
                throw new Exception("Shipment not found");
            }
            shipment.Cancel(request.reason);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
