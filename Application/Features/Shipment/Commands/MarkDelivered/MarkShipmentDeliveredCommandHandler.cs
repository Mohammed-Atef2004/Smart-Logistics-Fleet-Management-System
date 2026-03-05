using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.MarkDelivered
{
    public class MarkShipmentDeliveredCommandHandler: IRequestHandler<MarkShipmentDeliveredCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public MarkShipmentDeliveredCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(MarkShipmentDeliveredCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.Shipments.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.MarkDelivered(request.deliveredAt, request.receivedBy);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}
