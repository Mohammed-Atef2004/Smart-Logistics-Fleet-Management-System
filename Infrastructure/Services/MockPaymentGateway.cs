using Domain.Payments;

namespace Infrastructure.Services
{
    // ده mock implementation - في production هتستبدله بـ Stripe أو Paymob أو أي gateway
    // بس الـ interface موجود في الـ Application layer وده هو الصح
    public class MockPaymentGateway : IPaymentGateway
    {
        public async Task<PaymentGatewayResult> ProcessAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            // في production هنا بتعمل HTTP call للـ gateway الحقيقي
            await Task.Delay(100, cancellationToken); // simulate network call

            // للـ testing: لو الـ amount بيتقسم على 13 بيفشل 😄
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
