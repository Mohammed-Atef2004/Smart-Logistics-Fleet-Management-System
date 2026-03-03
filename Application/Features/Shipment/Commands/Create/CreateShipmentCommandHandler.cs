using Application.Features.Shipments.ValueObjects;
using Domain.Shipments;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.Create
{
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<ShipmentId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateShipmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.Shipments.AddAsync(shipment.Value);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result<ShipmentId>.Success(shipment.Value.Id);
        }
    }
}
