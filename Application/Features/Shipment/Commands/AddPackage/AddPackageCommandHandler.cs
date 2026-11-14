using Domain.Shipments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Commands.AddPackage
{
    public sealed class AddPackageCommandHandler : IRequestHandler<AddPackageCommand, Result<PackageId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipmentRepository _shipmentRepository;
        public AddPackageCommandHandler(IUnitOfWork unitOfWork, IShipmentRepository shipmentRepository)
        {
            _unitOfWork = unitOfWork;
            _shipmentRepository = shipmentRepository;
        }
        public async Task<Result<PackageId>> Handle(AddPackageCommand request, CancellationToken cancellationToken)
        {
            var shipmentResult = await _shipmentRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.ShipmentId);
            if (shipmentResult is null)
                return Result<PackageId>.Failure(new Error("Shipment.NotFound", "Shipment not found."));
            var shipment = shipmentResult;
            var packageResult = Domain.Shipments.Package.Create(
                description: request.Description,
                weight: Weight.Create(request.WeightValue, request.WeightUnit).Value,
                dimensions: Dimensions.Create(request.Length, request.Width, request.Height, request.DimensionUnit).Value,
                contentCategory: request.Category,
                isFragile: request.IsFragile,
                requiresRefrigeration: request.RequiresRefrigeration,
                declaredValue: request.DeclaredValue,
                currency: request.Currency
            );
            if (packageResult.IsFailure)
                return Result<PackageId>.Failure(packageResult.Error);
            var package = packageResult.Value;
            shipment.AddPackage(request.Description, package.Weight, package.Dimensions, package.ContentCategory, package.IsFragile, package.RequiresRefrigeration, package.DeclaredValue, package.Currency);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result<PackageId>.Success(package.Id);
        }
    }
}