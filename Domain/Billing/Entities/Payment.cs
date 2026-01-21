using Domain.Billing.ValueObjects;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing.Entities
{
    public class Payment : BaseEntity
    {
        public Guid InvoiceId { get; private set; }
        public Money Amount { get; private set; }
        public DateTime PaidAt { get; private set; }

        private Payment() { }

        public Payment(Guid invoiceId, Money amount)
        {
            InvoiceId = invoiceId;
            Amount = amount;
            PaidAt = DateTime.UtcNow;
        }
    }
}
