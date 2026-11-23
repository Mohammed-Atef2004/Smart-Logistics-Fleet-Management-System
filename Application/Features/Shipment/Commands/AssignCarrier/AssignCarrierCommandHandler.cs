using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.AssignCarrier;

public sealed class AssignCarrierCommandHandler : IRequestHandler<AssignCarrierCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public AssignCarrierCommandHandler(
        IUnitOfWork unitOfWork,
        IShipmentRepository shipmentRepository)
    {
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result> Handle(
        AssignCarrierCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        if (shipment is null)
            return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        // The old handler passed EstimatedDeliveryDate.ToString(), which
        // stored an empty string when the date was null.
        var result = shipment.AssignCarrier(request.CarrierName, request.EstimatedDeliveryDate);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
