using Domain.Drivers.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.HireDriver
{
    public class HireDriverCommandHandler:IRequestHandler<HireDriverCommand, Guid>
    {
      private readonly IUnitOfWork _unitOfWork;
        public HireDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.Drivers.AddAsync(driver);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return driver.Id.Value;
        }
    }
}
