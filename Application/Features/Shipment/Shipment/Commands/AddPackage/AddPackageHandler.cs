using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain.Shipment.Enums;

namespace Application.Features.Shipment.Shipment.Commands.AddPackage
{
    public class AddPackageHandler: IRequestHandler<AddPackageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddPackageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(AddPackageCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.guid);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {request.guid} not found.");
            }
            shipment.AddPackage(
                request.shipmentPackageDto.Weight,
                request.shipmentPackageDto.Description,
                (PackageType)request.shipmentPackageDto.packageType,
                request.shipmentPackageDto.DeclaredValue
            );
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
