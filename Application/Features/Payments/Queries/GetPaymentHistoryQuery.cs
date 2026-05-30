using Domain.Payments;
using Domain.Payments.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Queries;

public record GetPaymentHistoryQuery(
    Guid? InvoiceId = null,
    string? Status = null
) : IRequest<IReadOnlyList<PaymentListItemDto>>;

public class GetPaymentHistoryHandler
    : IRequestHandler<GetPaymentHistoryQuery, IReadOnlyList<PaymentListItemDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentHistoryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<IReadOnlyList<PaymentListItemDto>> Handle(
        GetPaymentHistoryQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Payment> payments = _paymentRepository.Query;

        // =========================
        // Filter by InvoiceId
        // =========================
        if (query.InvoiceId.HasValue)
        {
            payments = payments.Where(p =>
                p.InvoiceId.Id == query.InvoiceId.Value);
        }

        // =========================
        // Filter by Status
        // =========================
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<PaymentStatus>(query.Status, out var status))
        {
            payments = payments.Where(p => p.Status == status);
        }

        // =========================
        // Execute Query
        // =========================
        var result = await payments
            .OrderByDescending(p => p.Transaction!.ProcessedAt)
            .ToListAsync(cancellationToken);

        // =========================
        // Map to DTO
        // =========================
        return result
            .Select(p => new PaymentListItemDto(
                Id: p.Id.Value,
                InvoiceId: p.InvoiceId.Id,
                Amount: p.Amount,
                Status: p.Status.ToString(),
                PaymentMethodType: p.Method.Type,
                ProcessedAt: p.Transaction?.ProcessedAt
            ))
            .ToList()
            .AsReadOnly();
    }
}