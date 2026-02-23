using Domain.Drivers.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.UpdateLicence
{
    public class UpdateDriverLicenseCommandHandler : IRequestHandler<UpdateDriverLicenseCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateDriverLicenseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(UpdateDriverLicenseCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.DriverId);
            if (driver is null)
                throw new Exception("Driver Not Found");
            var licenseResult = DriverLicense.Create(request.LicenseNumber, request.ExpiryDate, request.Category);
            if (licenseResult.IsFailure)
                throw new Exception(licenseResult.Error.ToString());
            driver.UpdateLicense(licenseResult.Value);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
