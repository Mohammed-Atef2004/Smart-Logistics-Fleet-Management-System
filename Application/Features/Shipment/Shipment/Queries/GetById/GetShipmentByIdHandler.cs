using Application.Features.Shipment.Shipment.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Queries.GetShipmentById
{
    public class GetShipmentByIdHandler: IRequestHandler<GetShipmentByIdQuery, ShipmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetShipmentByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ShipmentDto> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.ShipmentRecords.GetByIdAsync(request.Id);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {request.Id} not found.");
            }
            var shipmentDto = _mapper.Map<ShipmentDto>(shipment);
            return shipmentDto;

        }
    }
}
