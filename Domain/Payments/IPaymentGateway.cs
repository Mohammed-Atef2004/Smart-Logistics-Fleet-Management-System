namespace Domain.Payments
{
    // The interface is defined here; the implementation lives in Infrastructure
    // Classic Dependency Inversion Principle
    public interface IPaymentGateway
    {
        Task<PaymentGatewayResult> ProcessAsync(Payment payment, CancellationToken cancellationToken = default);
    }

    public record PaymentGatewayResult(
        bool IsSuccess,
        string TransactionReference,
        string? FailureReason = null
    );
}
