using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Common;

namespace Domain.Fleet.Rules
{
    public class DriverMustBeActiveRule : IBusinessRule
    {
        private readonly bool _isActive;

        public DriverMustBeActiveRule(bool isActive)
        {
            _isActive = isActive;
        }

        public string Message => "Operation failed: The driver is currently inactive.";

        public bool IsBroken() => !_isActive;
    }
}