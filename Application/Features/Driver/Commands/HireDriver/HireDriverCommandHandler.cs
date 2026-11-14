using Application.Common.Interfaces;
using Domain.Drivers.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.HireDriver
{
    public sealed class HireDriverCommandHandler:IRequestHandler<HireDriverCommand, Guid>
    {
      private readonly IUnitOfWork _unitOfWork;
      private readonly IDriverRepository _driverRepository;
        public HireDriverCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _driverRepository = driverRepository;
        }
        public async Task<Guid> Handle(HireDriverCommand request, CancellationToken cancellationToken)
        {
            var licenseResult = DriverLicense.Create(request.LicenseNumber, request.ExpiryDate, request.Category);
            if (licenseResult.IsFailure)
                throw new Exception(licenseResult.Error.ToString());
            var driverResult = Domain.Drivers.Driver.Hire(request.FullName, licenseResult.Value);
            if (driverResult.IsFailure)
                throw new Exception(driverResult.Error.ToString());
            var driver = driverResult.Value;
            await _driverRepository.AddAsync(driver);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return driver.Id.Value;
        }
    }
}