using Application.Features.Driver.Dtos;
using AutoMapper;
using Domain.Fleet;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.Features.Driver.Commands.Create
{

    public class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateDriverCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Guid> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
            var driverEntity = _mapper.Map<Domain.Fleet.Driver>(request.driverDto);

            await _unitOfWork.Drivers.AddAsync(driverEntity);

            await _unitOfWork.CompleteAsync(cancellationToken);

            return driverEntity.Id;
        }
    }
}

