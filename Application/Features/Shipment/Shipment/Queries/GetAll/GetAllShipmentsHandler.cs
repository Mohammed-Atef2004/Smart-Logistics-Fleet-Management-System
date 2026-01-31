using Application.Features.Shipment.Shipment.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Queries.GetAll
{
    public class GetAllShipmentsHandler: IRequestHandler<GetAllShipmentsQuery, List<ShipmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllShipmentsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<ShipmentDto>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments =  _unitOfWork.ShipmentRecords.EntityQuery;
            shipments =  shipments.Select(x=>x);
            var shipmentDtos = _mapper.Map<List<ShipmentDto>>(shipments);
            return shipmentDtos;
        }
    }
}
