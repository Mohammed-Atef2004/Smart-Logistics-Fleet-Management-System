using Application.Common.Interfaces;
using Application.Features.Shipment.DTOs;
using AutoMapper;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Queries.GetAll
{
    public sealed class GetAllShipmentsQueryHandler : IRequestHandler<GetAllShipmentsQuery, Result<List<ShipmentListDto>>>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;
        public GetAllShipmentsQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public Task<Result<List<ShipmentListDto>>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = _shipmentRepository.EntityQuery.Select(x => x);
            var shipmentDtos = _mapper.ProjectTo<ShipmentListDto>(shipments);
            return Task.FromResult(Result<List<ShipmentListDto>>.Success(shipmentDtos.ToList()));
        }
    }
}