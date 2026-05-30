using Domain.Invoices.ValueObjects;
using Domain.Payments.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Payments.Events
{
    // لما الـ payment يتعالج بنجاح - الـ InvoiceId محتاجينه عشان نعمل
    // cross-aggregate communication ونعمل mark على الـ Invoice إنه اتدفع
    public record PaymentProcessedEvent(
        PaymentId PaymentId,
        InvoiceId InvoiceId,
        decimal Amount) : DomainEvent;
}
