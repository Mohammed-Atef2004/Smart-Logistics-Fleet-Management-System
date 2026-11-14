using Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.Dispatch
{
    public sealed class DispatchCommandHandler : IRequestHandler<DispatchCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public DispatchCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(DispatchCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x=>x.Id==request.Id);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.Dispatch();
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}