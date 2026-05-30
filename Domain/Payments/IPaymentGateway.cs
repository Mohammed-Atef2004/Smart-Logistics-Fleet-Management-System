namespace Domain.Payments
{
    // الـ interface هنا في الـ Application
    // الـ implementation في الـ Infrastructure
    // Dependency Inversion Principle بالظبط
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
