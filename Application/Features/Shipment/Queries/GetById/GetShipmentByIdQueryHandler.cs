using Application.Features.Shipment.DTOs;
using Domain.Shipments;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Queries.GetById;

public sealed class GetShipmentByIdQueryHandler
    : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentDetailsDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IMapper _mapper;

    public GetShipmentByIdQueryHandler(
        IShipmentRepository shipmentRepository,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<ShipmentDetailsDto>> Handle(
        GetShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId), cancellationToken);

        // The old handler returned null! for a missing shipment, so the
        // controller threw a NullReferenceException instead of returning 404.
        if (shipment is null)
            return Result<ShipmentDetailsDto>.Failure(
                ShipmentErrors.NotFound(request.ShipmentId));

        return Result<ShipmentDetailsDto>.Success(
            _mapper.Map<ShipmentDetailsDto>(shipment));
    }
}
