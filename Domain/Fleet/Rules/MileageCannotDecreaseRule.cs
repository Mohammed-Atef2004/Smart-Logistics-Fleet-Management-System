using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Fleet.Rules
{
    public class MileageCannotDecreaseRule: IBusinessRule
    {
        private readonly int _currentMileage;
        private readonly int _newMileage;
        public MileageCannotDecreaseRule(int currentMileage, int newMileage)
        {
            _currentMileage = currentMileage;
            _newMileage = newMileage;
        }
        public string Message => "Mileage cannot decrease.";
        public bool IsBroken() => _newMileage < _currentMileage;
    }
}
