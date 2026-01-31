using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.CompleteDelivery
{
    public class CompleteDeliveryHandler: IRequestHandler<CompleteDeliveryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompleteDeliveryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(CompleteDeliveryCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.Id);
            
            if (shipment == null)
            {
                throw new Exception("Shipment not found");
            }
            shipment.CompleteDelivery();
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
