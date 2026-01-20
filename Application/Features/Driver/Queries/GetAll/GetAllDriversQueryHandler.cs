using Application.Features.Driver.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Queries.GetAll
{
    public class GetAllDriversQueryHandler:IRequestHandler<GetAllDriversQuery,List<DriverDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllDriversQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<DriverDto>>Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
        {
            var result=await _unitOfWork.Drivers.GetAllAsync();
           var drivers= _mapper.Map<List<DriverDto>>(result);
            return drivers;
        }
    }
}
