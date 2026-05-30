using Domain.Invoices.ValueObjects;
using Domain.Payments.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Payments.Events
{
    public record PaymentFailedEvent(
        PaymentId PaymentId,
        InvoiceId InvoiceId,
        string Reason) : DomainEvent;
}
