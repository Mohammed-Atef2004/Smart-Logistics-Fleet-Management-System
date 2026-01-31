using Application.Features.Shipment.Shipment.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Queries.GetDelayed
{
    public class GetDelayedShipmentsHandler:IRequestHandler<GetDelayedShipmentsQuery, List<ShipmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetDelayedShipmentsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<ShipmentDto>> Handle(GetDelayedShipmentsQuery request, CancellationToken cancellationToken)
        {
            var delayedShipments =  _unitOfWork.ShipmentRecords.EntityQuery;
            if (delayedShipments == null)
                return new List<ShipmentDto>();
            delayedShipments = delayedShipments.Where(s => s.EstimatedDeliveryDate < DateTime.UtcNow && s.Status != Domain.Shipment.Enums.ShipmentStatus.Delivered);
            return _mapper.Map<List<ShipmentDto>>(delayedShipments);
        }
    }
}
