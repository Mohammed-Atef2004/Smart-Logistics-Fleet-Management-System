using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.UpdateDeliveryAddress;

public sealed class UpdateShipmentDeliveryAddressCommandHandler
    : IRequestHandler<UpdateShipmentDeliveryAddressCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public UpdateShipmentDeliveryAddressCommandHandler(
        IUnitOfWork unitOfWork,
        IShipmentRepository shipmentRepository)
    {
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result> Handle(
        UpdateShipmentDeliveryAddressCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        if (shipment is null)
            return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        var addressResult = DeliveryAddress.Create(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.ApartmentUnit);

        if (addressResult.IsFailure)
            return Result.Failure(addressResult.Error);

        var result = shipment.UpdateDeliveryAddress(addressResult.Value);
        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success();
    }
}
