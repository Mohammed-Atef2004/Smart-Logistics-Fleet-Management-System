using Application.Features.Driver.Dtos;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Interfaces.Repositories;

namespace Application.Features.Driver.Queries.GetById
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
           var result=_unitOfWork.Drivers.GetByIdAsync(request.Id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Driver with Id {request.Id} not found.");
            }
            var driver = _mapper.Map<DriverDto>(result);
            return await Task.FromResult(driver);
        }
    }
}
