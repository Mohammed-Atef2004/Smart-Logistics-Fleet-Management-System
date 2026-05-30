using Domain.Payments.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Payments.Events
{
    public record PaymentRefundedEvent(
        PaymentId PaymentId,
        decimal Amount) : DomainEvent;
}
