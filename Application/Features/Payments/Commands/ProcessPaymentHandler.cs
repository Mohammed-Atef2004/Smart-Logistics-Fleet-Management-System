using Domain.Interfaces.Repositories;
using Domain.Invoices;
using Domain.Invoices.ValueObjects;
using Domain.Payments;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments.Commands.ProcessPayment;

public class ProcessPaymentHandler
    : IRequestHandler<ProcessPaymentCommand, Result<Guid>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.EntityQuery
            .FirstOrDefaultAsync(
                x => x.Id == new InvoiceId(command.InvoiceId),
                cancellationToken);

        if (invoice is null)
            return Result<Guid>.Failure(InvoiceErrors.NotFound);

        var methodResult = command.PaymentMethodType switch
        {
            "Cash" => PaymentMethod.Cash(),
            "BankTransfer" => PaymentMethod.BankTransfer(),
            "CreditCard" => PaymentMethod.CreditCard(
                command.Provider!,
                command.Last4Digits!),

            _ => Result<PaymentMethod>.Failure(
                new Error("Payment.InvalidMethod", "Invalid payment method"))
        };

        if (methodResult.IsFailure)
            return Result<Guid>.Failure(methodResult.Error);

        var paymentResult = Payment.Create(
            new InvoiceId(command.InvoiceId),
            command.Amount,
            methodResult.Value);

        if (paymentResult.IsFailure)
            return Result<Guid>.Failure(paymentResult.Error);

        var payment = paymentResult.Value;

        var gatewayResult = await _paymentGateway.ProcessAsync(
            payment,
            cancellationToken);

        if (gatewayResult.IsSuccess)
        {
            payment.Process(gatewayResult.TransactionReference);
        }
        else
        {
            payment.Fail(
                gatewayResult.TransactionReference,
                gatewayResult.FailureReason ?? "Unknown error");
        }

        await _paymentRepository.AddAsync(payment);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return payment.Status == Domain.Payments.Enums.PaymentStatus.Failed
            ? Result<Guid>.Failure(
                new Error(
                    "Payment.Failed",
                    gatewayResult.FailureReason ?? "Payment processing failed"))
            : Result<Guid>.Success(payment.Id.Value);
    }
}