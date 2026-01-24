using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Rules
{
    public class TotalWeightMustNotExceedLimitRule : IBusinessRule
    {
        private readonly double _totalWeight;
        private readonly double _maxWeight;

        public TotalWeightMustNotExceedLimitRule(double totalWeight, double maxWeight = 1000)
        {
            _totalWeight = totalWeight;
            _maxWeight = maxWeight;
        }

        public bool IsBroken() => _totalWeight > _maxWeight;

        public string Message => $"Total weight ({_totalWeight} kg) exceeds maximum allowed ({_maxWeight} kg)";
    }
}
