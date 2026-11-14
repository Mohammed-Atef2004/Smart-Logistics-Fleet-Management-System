using Domain.Shipments.ValueObjects;
using Domain.Shipments;

namespace Application.Features.Shipment.Commands.Create
{
    public sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<ShipmentId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public CreateShipmentCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result<ShipmentId>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
           
            var shipment = Domain.Shipments.Shipment.Create(
                senderId: request.SenderId,
                destinationAddress:  DeliveryAddress.Create(
                street: request.Street,
                city: request.City,
                state: request.State,
                zipCode: request.ZipCode,
                country: request.Country,
                apartmentUnit: request.ApartmentUnit),
                trackingNumber: request.TrackingNumber,
                priority: request.Priority,
                recipientName: request.RecipientName,
                recipientPhone: request.RecipientPhone,
                specialInstructions: request.SpecialInstructions
            );
            if (shipment.IsFailure)
                return Result<ShipmentId>.Failure(shipment.Error);
            await _shipmentRepository.AddAsync(shipment.Value);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result<ShipmentId>.Success(shipment.Value.Id);
        }
    }
}