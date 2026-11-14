using Application.Common.Interfaces;
using Domain.Drivers.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.UpdateLicence
{
    public sealed class UpdateDriverLicenseCommandHandler : IRequestHandler<UpdateDriverLicenseCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverRepository _driverRepository;
        public UpdateDriverLicenseCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _driverRepository = driverRepository;
        }
        public async Task<Unit> Handle(UpdateDriverLicenseCommand request, CancellationToken cancellationToken)
        {
            var driver = await _driverRepository.EntityQuery.SingleOrDefaultAsync(d => d.Id == request.DriverId);
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