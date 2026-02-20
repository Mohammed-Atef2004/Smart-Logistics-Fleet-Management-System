using Domain.Drivers.Errors;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers.ValueObjects
{
    public record DriverLicense
    {
        public string LicenseNumber { get; }
        public DateTime ExpiryDate { get; }
        public string Category { get; } 

        private DriverLicense(string number, DateTime expiry, string category)
        {
            LicenseNumber = number;
            ExpiryDate = expiry;
            Category = category;
        }
        public DriverLicense() { }

        public static Result<DriverLicense> Create(string number, DateTime expiry, string category)
        {
            if (string.IsNullOrWhiteSpace(number))
                return Result<DriverLicense>.Failure(DriverErrors.InvalidLicenceNumber);

            if (expiry < DateTime.UtcNow)
                return Result<DriverLicense>.Failure(DriverErrors.LicenseExpired);

            return Result<DriverLicense>.Success(new DriverLicense(number, expiry, category));
        }

        public bool IsExpired() => ExpiryDate < DateTime.UtcNow;
    }
}
