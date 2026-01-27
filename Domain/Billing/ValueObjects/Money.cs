using Domain.Common;
using Domain.Common.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Domain.Billing.ValueObjects
{
    [Owned]
    public class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
        private Money() { } // For EF
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
