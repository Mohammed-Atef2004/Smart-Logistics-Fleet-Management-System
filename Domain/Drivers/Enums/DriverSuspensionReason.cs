using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers.Enums
{
    public enum DriverSuspensionReason
    {
        LowRating,
        Fraud,
        LicenseExpired,
        AdminAction
    }
}
