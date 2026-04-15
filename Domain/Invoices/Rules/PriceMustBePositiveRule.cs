using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Invoices.Rules
{
    public class PriceMustBePositiveRule : IBusinessRule
    {
        private double _price;

        public PriceMustBePositiveRule(double price)
        {
            _price = price;
        }

        public Error Error => new("Price.Value", "Price Must Be Positive Value");

        public bool IsBroken() => _price < 0;
    }
}
