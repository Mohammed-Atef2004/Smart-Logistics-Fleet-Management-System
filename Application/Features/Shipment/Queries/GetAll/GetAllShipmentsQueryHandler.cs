using Application.Features.Shipment.DTOs;
using Domain.Shipments;

namespace Application.Features.Shipment.Queries.GetAll;

public sealed class GetAllShipmentsQueryHandler
    : IRequestHandler<GetAllShipmentsQuery, Result<List<ShipmentListDto>>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IMapper _mapper;

    public GetAllShipmentsQueryHandler(
        IShipmentRepository shipmentRepository,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ShipmentListDto>>> Handle(
        GetAllShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        // The old handler called ProjectTo(...).ToList() on a deferred query
        // inside a synchronous Task.FromResult; the repository now does the
        // async materialization so the Application layer stays EF-free.
        var shipments = await _shipmentRepository.ListAsync(request.Status, cancellationToken);

        return Result<List<ShipmentListDto>>.Success(
            _mapper.Map<List<ShipmentListDto>>(shipments));
    }
}
