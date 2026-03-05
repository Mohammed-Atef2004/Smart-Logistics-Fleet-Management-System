using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.MarkDeliveryFailed
{
    public class MarkShipmentDeliveryFailedCommandHandler : IRequestHandler<MarkShipmentDeliveryFailedCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public MarkShipmentDeliveryFailedCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(MarkShipmentDeliveryFailedCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.Shipments.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.MarkDeliveryFailed(request.reason);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}
