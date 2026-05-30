using Domain.Payments;
using Domain.Payments.Enums;
using Domain.Payments.ValueObjects;

namespace Domain.Payments;

public interface IPaymentRepository
{
    IQueryable<Payment> Query { get; }

    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken cancellationToken = default);

    Task<Payment?> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);

    Task<List<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);

    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);

    void Update(Payment payment);

    void Delete(Payment payment);
}