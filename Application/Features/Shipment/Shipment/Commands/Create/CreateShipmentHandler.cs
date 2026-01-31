using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.Create
{
  
    public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateShipmentHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid>  Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var result=_mapper.Map<Domain.Shipment.Entities.Shipment>(request.shipmentDto);
            await _unitOfWork.ShipmentRecords.AddAsync(result);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return result.Id;
        }
    }
}
