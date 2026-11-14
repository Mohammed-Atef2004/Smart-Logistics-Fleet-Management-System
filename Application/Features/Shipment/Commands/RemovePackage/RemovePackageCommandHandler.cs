using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.RemovePackage
{
    public sealed class RemovePackageCommandHandler : IRequestHandler<RemovePackageCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public RemovePackageCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result> Handle(RemovePackageCommand request, CancellationToken cancellationToken)
        {
            var shipmentResult = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.ShipmentId);
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