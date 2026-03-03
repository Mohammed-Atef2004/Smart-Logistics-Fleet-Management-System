using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.AssignCarrier
{
    internal class AssignCarrierCommandHandler : IRequestHandler<AssignCarrierCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AssignCarrierCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(AssignCarrierCommand request, CancellationToken cancellationToken)
        {
            var shipmentResult = await _unitOfWork.Shipments.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.ShipmentId);
            if (shipmentResult is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            var shipment = shipmentResult;
            shipment.AssignCarrier(request.CarrierName, request.EstimatedDeliveryDate.ToString());
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}
