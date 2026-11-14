using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.AssignCarrier
{
    internal sealed class AssignCarrierCommandHandler : IRequestHandler<AssignCarrierCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public AssignCarrierCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(AssignCarrierCommand request, CancellationToken cancellationToken)
        {
            var shipmentResult = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.ShipmentId);
            if (shipmentResult is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            var shipment = shipmentResult;
            shipment.AssignCarrier(request.CarrierName, request.EstimatedDeliveryDate.ToString());
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}