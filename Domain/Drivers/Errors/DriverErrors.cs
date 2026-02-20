using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers.Errors
{
    public static class DriverErrors
    {
        public static Error EmptyName = new("Driver.EmptyName", "Driver name is required.");
        public static Error NotActive = new("Driver.NotActive", "Driver must be active.");
        public static Error AlreadySuspended = new("Driver.AlreadySuspended", "Driver already suspended.");
        public static Error LicenseExpired = new("Driver.LicenseExpired", "Driver license is expired.");
        public static Error ShiftAlreadyActive = new("Driver.ShiftAlreadyActive", "Driver already in shift.");
        public static Error InvalidLicenceNumber = new("Driver.InvalidLicenseNumber", "Driver license number is invalid.");
        public static Error AlreadyInShift = new("Driver.AlreadyInShift", "Driver is already assigned to a shift.");
        public static Error InvalidRating = new("Driver.InvalidRating", "Driver rating must be between 1 and 5.");
        public static Error DriverNotActive = new("Driver.NotActive", "Driver must be active to perform this action.");
    }
}
