using Domain.SharedKernel;

namespace Domain.Payments.ValueObjects
{
    public record PaymentId(Guid Value)
    {
        public static PaymentId New() => new(Guid.NewGuid());

        public override string ToString() => Value.ToString();
    }
}
