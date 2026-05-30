using FluentValidation;

namespace Application.Payments.Commands.ProcessPayment
{
    public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("Invoice Id is required");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.PaymentMethodType)
                .NotEmpty().WithMessage("Payment method is required")
                .Must(x => new[] { "CreditCard", "BankTransfer", "Cash" }.Contains(x))
                .WithMessage("Payment method must be CreditCard, BankTransfer, or Cash");

            // لو CreditCard لازم يبعت الـ provider
            When(x => x.PaymentMethodType == "CreditCard", () =>
            {
                RuleFor(x => x.Provider)
                    .NotEmpty().WithMessage("Provider is required for credit card payments");

                RuleFor(x => x.Last4Digits)
                    .NotEmpty().WithMessage("Last 4 digits are required for credit card payments")
                    .Length(4).WithMessage("Last 4 digits must be exactly 4 characters");
            });
        }
    }
}
