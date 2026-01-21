using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.Update
{
    public class UpdateDriverCommandHandler:IRequestHandler<UpdateDriversCommand,Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateDriverCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateDriversCommand request, CancellationToken cancellationToken)
        {
            var result = _unitOfWork.Drivers.GetByIdAsync(request.Id);
            var driver=result.Result;
            if (driver == null)
            {
                throw new NullReferenceException();
            }
            driver.Update(request.driverDto.FullName, request.driverDto.LicenseNumber);
            _unitOfWork.Drivers.Update(driver);
            _unitOfWork.CompleteAsync();
            return Unit.Value;
        }
    }
}
