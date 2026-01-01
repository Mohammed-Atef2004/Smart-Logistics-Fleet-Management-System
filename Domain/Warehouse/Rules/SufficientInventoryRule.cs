using Domain.Common;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

namespace Domain.Warehouse.Rules
{

    public class SufficientInventoryRule : IBusinessRule
    {

        private readonly int _current;

        private readonly int _amount;

        public SufficientInventoryRule(int current, int amount)

        {

            _current = current;

            _amount = amount;

        }

        public bool IsBroken() => _current < _amount;

        public string Message => "Insufficient inventory.";

    }

}

