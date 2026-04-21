using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Invoices.Events
{
    public record InvoicePaidEvent(InvoiceId InvoiceId) : DomainEvent;
}
