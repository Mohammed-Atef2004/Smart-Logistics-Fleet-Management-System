using Domain.SharedKernel;

namespace Domain.Payments
{
    public record PaymentMethod
    {
        public string Type { get; init; }       // e.g. "CreditCard", "BankTransfer", "Cash"
        public string? Provider { get; init; }  // e.g. "Visa", "MasterCard" - optional
        public string? Last4Digits { get; init; } // optional للكارت

        private PaymentMethod() { }

        private PaymentMethod(string type, string? provider, string? last4Digits)
        {
            Type = type;
            Provider = provider;
            Last4Digits = last4Digits;
        }

        public static Result<PaymentMethod> Create(string type, string? provider = null, string? last4Digits = null)
        {
            if (string.IsNullOrWhiteSpace(type))
                return Result<PaymentMethod>.Failure(new Error("PaymentMethod.InvalidType", "Payment method type is required"));

            var allowed = new[] { "CreditCard", "BankTransfer", "Cash" };
            if (!allowed.Contains(type))
                return Result<PaymentMethod>.Failure(new Error("PaymentMethod.InvalidType", $"Payment method must be one of: {string.Join(", ", allowed)}"));

            if (type == "CreditCard" && last4Digits != null && last4Digits.Length != 4)
                return Result<PaymentMethod>.Failure(new Error("PaymentMethod.InvalidCard", "Last 4 digits must be exactly 4 characters"));

            return Result<PaymentMethod>.Success(new PaymentMethod(type, provider, last4Digits));
        }

        // Factory Methods للـ convenience
        public static Result<PaymentMethod> Cash() => Create("Cash");
        public static Result<PaymentMethod> BankTransfer() => Create("BankTransfer");
        public static Result<PaymentMethod> CreditCard(string provider, string last4Digits) => Create("CreditCard", provider, last4Digits);
    }
}
