using Application.Features.Driver.DTOs;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Queries.GetById
{
    public class GetDriverByIdQueryHandler : IRequestHandler<GetDriverByIdQuery, DriverDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDriverByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DriverDto> Handle(GetDriverByIdQuery request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.Id);
            if (driver is null)
                throw new Exception("Driver Not Found");
            return _mapper.Map<DriverDto>(driver);

        }
    }
}