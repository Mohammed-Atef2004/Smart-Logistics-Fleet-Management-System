using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing.Events
{
    public class InvoicePaidEvent : DomainEvent
    {
        public Guid InvoiceId { get; }

        public InvoicePaidEvent(Guid invoiceId)
        {
            InvoiceId = invoiceId;
        }
    }
}
