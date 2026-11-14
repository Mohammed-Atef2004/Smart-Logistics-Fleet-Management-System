using Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.MarkDelivered
{
    public sealed class MarkShipmentDeliveredCommandHandler: IRequestHandler<MarkShipmentDeliveredCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public MarkShipmentDeliveredCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(MarkShipmentDeliveredCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.MarkDelivered(request.deliveredAt, request.receivedBy);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}