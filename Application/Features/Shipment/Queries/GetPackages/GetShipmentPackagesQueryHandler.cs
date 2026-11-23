using Application.Features.Shipment.DTOs;
using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Queries.GetPackages;

public sealed class GetShipmentPackagesQueryHandler
    : IRequestHandler<GetShipmentPackagesQuery, Result<List<PackageDto>>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IMapper _mapper;

    public GetShipmentPackagesQueryHandler(
        IShipmentRepository shipmentRepository,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PackageDto>>> Handle(
        GetShipmentPackagesQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        // A missing shipment and a shipment with no packages are different
        // outcomes: the first is 404, the second is an empty list. The old
        // handler returned "Shipment.NotFound" for both.
        if (shipment is null)
            return Result<List<PackageDto>>.Failure(
                ShipmentErrors.NotFound(request.ShipmentId));

        return Result<List<PackageDto>>.Success(
            _mapper.Map<List<PackageDto>>(shipment.Packages));
    }
}
