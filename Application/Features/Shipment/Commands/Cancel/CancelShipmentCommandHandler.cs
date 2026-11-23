using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.Cancel;

public sealed class CancelShipmentCommandHandler : IRequestHandler<CancelShipmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public CancelShipmentCommandHandler(
        IUnitOfWork unitOfWork,
        IShipmentRepository shipmentRepository)
    {
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result> Handle(
        CancelShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        if (shipment is null)
            return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        var result = shipment.Cancel(request.Reason, request.CancelledBy);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
