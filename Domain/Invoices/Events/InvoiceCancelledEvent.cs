using Domain.Invoices.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Invoices.Events
{
    public record InvoiceCancelledEvent(InvoiceId InvoiceId) : DomainEvent;
}
