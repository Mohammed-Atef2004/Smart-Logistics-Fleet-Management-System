using Domain.Payments;

namespace Infrastructure.Services
{
    // Mock implementation - in production replace with Stripe, Paymob or another gateway
    // The interface lives in the Application layer, which is the correct placement
    public class MockPaymentGateway : IPaymentGateway
    {
        public async Task<PaymentGatewayResult> ProcessAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            // In production this is where the HTTP call to the real gateway goes
            await Task.Delay(100, cancellationToken); // simulate network call

            // For testing: fails when the amount is divisible by 13
            if (payment.Amount % 13 == 0)
            {
                return new PaymentGatewayResult(
                    IsSuccess: false,
                    TransactionReference: $"TXN-FAILED-{Guid.NewGuid():N}",
                    FailureReason: "Insufficient funds"
                );
            }

            return new PaymentGatewayResult(
                IsSuccess: true,
                TransactionReference: $"TXN-{Guid.NewGuid():N}"
            );
        }
    }
}
