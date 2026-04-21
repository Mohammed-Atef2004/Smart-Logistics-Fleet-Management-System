using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Invoices.Events
{
    public record InvoiceCreatedEvent(
        InvoiceId id,
        decimal Price
        ):DomainEvent;
}
