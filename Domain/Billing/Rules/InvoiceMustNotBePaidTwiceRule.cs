using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing.Rules
{
    public class InvoiceMustNotBePaidTwiceRule : IBusinessRule
    {
        private readonly bool _isPaid;

        public InvoiceMustNotBePaidTwiceRule(bool isPaid)
        {
            _isPaid = isPaid;
        }

        public bool IsBroken() => _isPaid;

        public string Message => "Invoice already paid.";
    }
}

