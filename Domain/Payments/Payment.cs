using Domain.Invoices.ValueObjects;
using Domain.Payments.Enums;
using Domain.Payments.Errors;
using Domain.Payments.Events;
using Domain.Payments.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Payments
{
    public class Payment : AggregateRoot<PaymentId>
    {
        public InvoiceId InvoiceId { get; private set; }

        public decimal Amount { get; private set; }

        public PaymentMethod Method { get; private set; }

        public PaymentStatus Status { get; private set; }

        public TransactionInfo? Transaction { get; private set; } // بيبقى null لو لسه Pending

        // EF Core
        private Payment() { }

        private Payment(PaymentId id, InvoiceId invoiceId, decimal amount, PaymentMethod method)
        {
            Id = id;
            InvoiceId = invoiceId;
            Amount = amount;
            Method = method;
            Status = PaymentStatus.Pending;
        }

        // -------------------------
        // Factory Method
        // -------------------------

        public static Result<Payment> Create(InvoiceId invoiceId, decimal amount, PaymentMethod method)
        {
            if (amount <= 0)
                return Result<Payment>.Failure(PaymentErrors.AmountMustBePositive);

            var payment = new Payment(PaymentId.New(), invoiceId, amount, method);

            // مفيش event هنا - الـ payment لسه مش اتعالج، بس اتعمل
            return Result<Payment>.Success(payment);
        }

        // -------------------------
        // Behavior
        // -------------------------

        // لما الـ payment gateway يرجع success
        public Result Process(string transactionReference)
        {
            if (Status != PaymentStatus.Pending)
                return Result.Failure(PaymentErrors.NotPending);

            if (string.IsNullOrWhiteSpace(transactionReference))
                return Result.Failure(new Error("Payment.InvalidReference", "Transaction reference is required"));

            Status = PaymentStatus.Processed;
            Transaction = TransactionInfo.ForSuccess(transactionReference, DateTime.UtcNow);

            AddDomainEvent(new PaymentProcessedEvent(Id, InvoiceId, Amount));

            return Result.Success();
        }

        // لما الـ payment gateway يرجع failure
        public Result Fail(string transactionReference, string reason)
        {
            if (Status != PaymentStatus.Pending)
                return Result.Failure(PaymentErrors.NotPending);

            if (string.IsNullOrWhiteSpace(reason))
                return Result.Failure(new Error("Payment.InvalidReason", "Failure reason is required"));

            Status = PaymentStatus.Failed;
            Transaction = TransactionInfo.ForFailure(transactionReference, DateTime.UtcNow, reason);

            AddDomainEvent(new PaymentFailedEvent(Id, InvoiceId, reason));

            return Result.Success();
        }

        // لما يطلبوا استرداد المبلغ
        public Result Refund()
        {
            if (Status != PaymentStatus.Processed)
                return Result.Failure(PaymentErrors.CannotRefundUnprocessed);

            Status = PaymentStatus.Refunded;

            AddDomainEvent(new PaymentRefundedEvent(Id, Amount));

            return Result.Success();
        }
    }
}
