using Domain.SharedKernel;

namespace Domain.Payments
{
    public record TransactionInfo
    {
        public string TransactionReference { get; init; }
        public DateTime ProcessedAt { get; init; }
        public string? FailureReason { get; init; } 

        private TransactionInfo() { }

        private TransactionInfo(string transactionReference, DateTime processedAt, string? failureReason)
        {
            TransactionReference = transactionReference;
            ProcessedAt = processedAt;
            FailureReason = failureReason;
        }

        public static TransactionInfo ForSuccess(string transactionReference, DateTime processedAt)
            => new(transactionReference, processedAt, null);

        public static TransactionInfo ForFailure(string transactionReference, DateTime processedAt, string failureReason)
            => new(transactionReference, processedAt, failureReason);
    }
}
