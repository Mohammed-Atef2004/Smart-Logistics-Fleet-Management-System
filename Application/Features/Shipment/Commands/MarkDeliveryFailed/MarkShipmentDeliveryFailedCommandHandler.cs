using Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.MarkDeliveryFailed
{
    public sealed class MarkShipmentDeliveryFailedCommandHandler : IRequestHandler<MarkShipmentDeliveryFailedCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public MarkShipmentDeliveryFailedCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(MarkShipmentDeliveryFailedCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.MarkDeliveryFailed(request.reason);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}