using Domain.Payments;
using Domain.Payments.Errors;
using Domain.Payments.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Queries;

// =====================
// Query
// =====================
public record GetPaymentDetailsQuery(Guid PaymentId)
    : IRequest<Result<PaymentDetailsDto>>;

// =====================
// Handler
// =====================
public class GetPaymentDetailsHandler
    : IRequestHandler<GetPaymentDetailsQuery, Result<PaymentDetailsDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentDetailsHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<PaymentDetailsDto>> Handle(
        GetPaymentDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.Query
            .FirstOrDefaultAsync(
                x => x.Id == new PaymentId(query.PaymentId),
                cancellationToken);

        if (payment is null)
            return Result<PaymentDetailsDto>.Failure(PaymentErrors.NotFound);

        var dto = new PaymentDetailsDto(
            Id: payment.Id.Value,
            InvoiceId: payment.InvoiceId.Id,
            Amount: payment.Amount,
            Status: payment.Status.ToString(),
            PaymentMethodType: payment.Method.Type,
            Provider: payment.Method.Provider,
            Last4Digits: payment.Method.Last4Digits,
            TransactionReference: payment.Transaction?.TransactionReference,
            ProcessedAt: payment.Transaction?.ProcessedAt,
            FailureReason: payment.Transaction?.FailureReason
        );

        return Result<PaymentDetailsDto>.Success(dto);
    }
}