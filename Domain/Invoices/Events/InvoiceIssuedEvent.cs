using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Invoices.Events
{
    public record InvoiceIssuedEvent(InvoiceId InvoiceId, decimal TotalPrice) : DomainEvent;
}
