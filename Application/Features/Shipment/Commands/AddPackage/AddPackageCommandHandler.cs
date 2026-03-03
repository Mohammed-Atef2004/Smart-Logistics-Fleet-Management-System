using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.AddPackage
{
    public class AddPackageCommandHandler : IRequestHandler<AddPackageCommand, Result<PackageId>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddPackageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<PackageId>> Handle(AddPackageCommand request, CancellationToken cancellationToken)
        {
            var shipmentResult = await _unitOfWork.Shipments.EntityQuery.FirstOrDefaultAsync(x => x.Id == request.ShipmentId);
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
