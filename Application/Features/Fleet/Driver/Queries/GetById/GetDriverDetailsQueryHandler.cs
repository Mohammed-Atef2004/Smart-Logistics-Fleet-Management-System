using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Application.Features.Fleet.Driver.Dtos;

namespace Application.Features.Fleet.Driver.Queries.GetById
{
    public class GetDriverDetailsQueryHandler:IRequestHandler<GetDriverDetailsQuery, DriverDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetDriverDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<DriverDto> Handle(GetDriverDetailsQuery request, CancellationToken cancellationToken)
        {
            var driverEntity = await _unitOfWork.Drivers.GetByIdAsync(request.Id);

            if (driverEntity == null)
            {
                throw new KeyNotFoundException($"Driver with Id {request.Id} not found.");
            }

            var driverDto = _mapper.Map<DriverDto>(driverEntity);

            return driverDto;
        }
    }
}
