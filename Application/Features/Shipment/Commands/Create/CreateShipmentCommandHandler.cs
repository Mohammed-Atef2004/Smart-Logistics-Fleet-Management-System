using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;
using ShipmentAggregate = Domain.Shipments.Shipment;

namespace Application.Features.Shipment.Commands.Create;

public sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public CreateShipmentCommandHandler(
        IUnitOfWork unitOfWork,
        IShipmentRepository shipmentRepository)
    {
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        // DeliveryAddress.Create used to throw, so an invalid body produced a
        // 500. It now returns a domain error that maps to a 400.
        var addressResult = DeliveryAddress.Create(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.ApartmentUnit);

        if (addressResult.IsFailure)
            return Result<Guid>.Failure(addressResult.Error);

        var trackingNumber = (request.TrackingNumber ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result<Guid>.Failure(ShipmentErrors.EmptyTrackingNumber);

        // The repository already exposed this check but nothing called it, so
        // duplicate tracking numbers were accepted and broke lookups.
        if (await _shipmentRepository.TrackingNumberExistsAsync(trackingNumber, cancellationToken))
            return Result<Guid>.Failure(ShipmentErrors.DuplicateTrackingNumber(trackingNumber));

        var shipmentResult = ShipmentAggregate.Create(
            request.SenderId,
            addressResult.Value,
            trackingNumber,
            request.Priority,
            request.RecipientName,
            request.RecipientPhone,
            request.SpecialInstructions);

        if (shipmentResult.IsFailure)
            return Result<Guid>.Failure(shipmentResult.Error);

        await _shipmentRepository.AddAsync(shipmentResult.Value);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(shipmentResult.Value.Id.Value);
    }
}
