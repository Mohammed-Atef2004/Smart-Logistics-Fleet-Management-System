using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.RemovePackage
{
    public class RemovePackageCommandHandler : IRequestHandler<RemovePackageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemovePackageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(RemovePackageCommand request, CancellationToken cancellationToken)
        {
            var shipmentResult = await _unitOfWork.Shipments.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.ShipmentId);
            if (shipmentResult is null)
                return Result.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            var shipment = shipmentResult;
            var package = shipment.Packages.FirstOrDefault(p => p.Id == request.PackageId);
            if (package is null)
                return Result.Failure(new Error("Package.NotFound", "Package not found in the specified shipment."));
            shipment.RemovePackage(package.Id);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
    }
}
