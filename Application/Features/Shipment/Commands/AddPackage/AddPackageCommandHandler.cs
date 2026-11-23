using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;
using ShipmentAggregate = Domain.Shipments.Shipment;

namespace Application.Features.Shipment.Commands.AddPackage;

public sealed class AddPackageCommandHandler : IRequestHandler<AddPackageCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShipmentRepository _shipmentRepository;

    public AddPackageCommandHandler(
        IUnitOfWork unitOfWork,
        IShipmentRepository shipmentRepository)
    {
        _unitOfWork = unitOfWork;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result<Guid>> Handle(
        AddPackageCommand request,
        CancellationToken cancellationToken)
    {
        // Tracked lookup: the old code read through AsNoTracking and then
        // mutated a detached aggregate, so the new package was never saved.
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        if (shipment is null)
            return Result<Guid>.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        var weightResult = Weight.Create(request.WeightValue, request.WeightUnit);
        if (weightResult.IsFailure)
            return Result<Guid>.Failure(weightResult.Error);

        var dimensionsResult = Dimensions.Create(
            request.Length, request.Width, request.Height, request.DimensionUnit);
        if (dimensionsResult.IsFailure)
            return Result<Guid>.Failure(dimensionsResult.Error);

        // The old handler created a package, then AddPackage created a second
        // one, and the handler returned the id of the discarded instance.
        var addResult = shipment.AddPackage(
            request.Description,
            weightResult.Value,
            dimensionsResult.Value,
            request.Category,
            request.IsFragile,
            request.RequiresRefrigeration,
            request.DeclaredValue,
            request.Currency);

        if (addResult.IsFailure)
            return Result<Guid>.Failure(addResult.Error);

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result<Guid>.Success(addResult.Value.Value);
    }
}
