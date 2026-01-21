using Domain.Common;
using Domain.Fleet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Fleet.Rules
{
    public class VehicleMustBeAvailableRule : IBusinessRule
    {
        private readonly VehicleStatus _status;
        public VehicleMustBeAvailableRule(VehicleStatus status)
        {
            _status = status;
        }
        public string Message => "The Vehicle Must be Available";

        bool IBusinessRule.IsBroken()=> _status != VehicleStatus.Available;

    }

}
