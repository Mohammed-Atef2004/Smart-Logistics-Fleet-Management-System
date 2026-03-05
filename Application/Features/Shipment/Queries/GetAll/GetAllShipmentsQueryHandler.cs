using Application.Features.Shipment.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
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
    public class GetAllShipmentsQueryHandler : IRequestHandler<GetAllShipmentsQuery, Result<List<ShipmentListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllShipmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<Result<List<ShipmentListDto>>> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = _unitOfWork.Shipments.EntityQuery.Select(x => x);
            var shipmentDtos = _mapper.ProjectTo<ShipmentListDto>(shipments);
            return Task.FromResult(Result<List<ShipmentListDto>>.Success(shipmentDtos.ToList()));
        }
    }
}
