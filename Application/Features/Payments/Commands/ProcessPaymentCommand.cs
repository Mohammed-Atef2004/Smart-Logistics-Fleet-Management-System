using Amazon.Runtime.Internal;
using Domain.SharedKernel;
using MediatR;

namespace Application.Payments.Commands.ProcessPayment
{
    public record ProcessPaymentCommand(
    Guid InvoiceId,
    decimal Amount,
    string PaymentMethodType,
    string? Provider,
    string? Last4Digits
) : IRequest<Result<Guid>>;          
}
