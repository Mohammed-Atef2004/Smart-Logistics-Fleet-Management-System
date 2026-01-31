using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repositories;

namespace Application.Features.Shipment.Shipment.Commands.RecordLocation
{
    public class RecordLocationHandler: IRequestHandler<RecordLocationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RecordLocationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(RecordLocationCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.Id);
            
            if (shipment == null)
            {
                throw new Exception("Shipment not found");
            }
            shipment.RecordLocation(request.location,request.notes);
            await _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
