using Domain.Shipments;
using Application.Features.Shipment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Shipment.Queries.GetById
{
    public sealed class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentDetailsDto>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;
        public GetShipmentByIdQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }
        public async Task<Result<ShipmentDetailsDto>> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.EntityQuery.Include(x => x.Packages).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return null!;
            return Result<ShipmentDetailsDto>.Success(_mapper.Map<ShipmentDetailsDto>(shipment));
        }
    }
}