using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repositories;

namespace Application.Features.Shipment.Shipment.Commands.MarkAsReturned
{
    public class MarkAsReturnedHandler: IRequestHandler<MarkAsReturnedCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public MarkAsReturnedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(MarkAsReturnedCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.Id);
            
            if (shipment == null)
            {
                throw new Exception("Shipment not found");
            }
            shipment.MarkAsReturned(request.readon);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
