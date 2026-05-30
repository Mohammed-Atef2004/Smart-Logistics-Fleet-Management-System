using Domain.SharedKernel;

namespace Domain.Payments.Errors
{
    public static class PaymentErrors
    {
        public static Error NotFound =>
            new("Payment.NotFound", "No Payment with This Id");

        public static Error AlreadyProcessed =>
            new("Payment.AlreadyProcessed", "Payment has already been processed");

        public static Error AmountMustBePositive =>
            new("Payment.InvalidAmount", "Payment amount must be greater than zero");

        public static Error NotPending =>
            new("Payment.NotPending", "Only pending payments can be processed or cancelled");

        public static Error CannotRefundUnprocessed =>
            new("Payment.CannotRefund", "Only processed payments can be refunded");
    }
}
