using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers.Rules
{
    public class DriverMustBeActiveRule : IBusinessRule
    {
        private readonly DriverStatus _status;
        public DriverMustBeActiveRule(DriverStatus status) => _status = status;

        public bool IsBroken() => _status != DriverStatus.Active;
        public Error Error => DriverErrors.DriverNotActive;
    }

}
