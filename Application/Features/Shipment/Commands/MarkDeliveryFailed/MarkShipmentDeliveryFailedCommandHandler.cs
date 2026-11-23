using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.MarkDeliveryFailed;

public sealed class MarkShipmentDeliveryFailedCommandHandler
    : IRequestHandler<MarkShipmentDeliveryFailedCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public MarkShipmentDeliveryFailedCommandHandler(
        IUnitOfWork unitOfWork,
        IShipmentRepository shipmentRepository)
    {
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result> Handle(
        MarkShipmentDeliveryFailedCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        if (shipment is null)
            return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        var result = shipment.MarkDeliveryFailed(request.Reason);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
