using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.AddRoutePoint
{
    public sealed class AddRoutePointCommandHandler:IRequestHandler<AddRoutePointCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public AddRoutePointCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(AddRoutePointCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.AddRoutePoint(request.location, request.description, request.arrivedAt, request.type);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}