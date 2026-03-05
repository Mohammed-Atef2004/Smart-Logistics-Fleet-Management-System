using Application.Features.Shipment.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Queries.GetById
{
    public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetShipmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ShipmentDetailsDto>> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            var shipment = await _unitOfWork.Shipments.EntityQuery.Include(x => x.Packages).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (shipment is null)
                return null!;
            return Result<ShipmentDetailsDto>.Success(_mapper.Map<ShipmentDetailsDto>(shipment));
        }
    }
}
