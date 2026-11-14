using Domain.Invoices.ValueObjects;
using Domain.Payments.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Payments.Events
{
    // Raised when the payment is processed successfully. The InvoiceId is
    // used for cross-aggregate communication to mark the invoice as paid
    public record PaymentProcessedEvent(
        PaymentId PaymentId,
        InvoiceId InvoiceId,
        decimal Amount) : DomainEvent;
}
