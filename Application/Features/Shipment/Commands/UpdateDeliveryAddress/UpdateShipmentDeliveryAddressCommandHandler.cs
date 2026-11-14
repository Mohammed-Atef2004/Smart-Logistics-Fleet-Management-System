using Application.Common.Interfaces;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.UpdateDeliveryAddress
{
    public sealed class UpdateShipmentDeliveryAddressCommandHandler: IRequestHandler<UpdateShipmentDeliveryAddressCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public UpdateShipmentDeliveryAddressCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(UpdateShipmentDeliveryAddressCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            shipment.UpdateDeliveryAddress(request.newAddress);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}