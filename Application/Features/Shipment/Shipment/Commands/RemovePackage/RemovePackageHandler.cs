using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.RemovePackage
{
    public class RemovePackageHandler: IRequestHandler<RemovePackageCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemovePackageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(RemovePackageCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.guid);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {request.guid} not found.");
            }
            shipment.RemovePackage(request.packageId);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
