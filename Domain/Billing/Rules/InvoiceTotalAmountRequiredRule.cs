using Domain.Billing.ValueObjects;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing.Rules
{
    public class InvoiceTotalAmountRequiredRule : IBusinessRule
    {
        private readonly Money _amount;

        public InvoiceTotalAmountRequiredRule(Money amount)
        {
            _amount = amount;
        }

        public bool IsBroken() => _amount == null;

        public string Message => "Total amount is required.";
    }
}
