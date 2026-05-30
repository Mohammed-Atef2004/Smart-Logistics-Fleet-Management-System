using Application.Common.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Payments;
using Domain.Payments.Errors;
using Domain.SharedKernel;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Payments.Commands.RefundPayment;

// =====================
// Command
// =====================
public record RefundPaymentCommand(Guid PaymentId) : IRequest<Result>;

// =====================
// Validator
// =====================
public class RefundPaymentValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment Id is required");
    }
}

// =====================
// Handler
// =====================
public class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RefundPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.Query
            .FirstOrDefaultAsync(
                x => x.Id == new Domain.Payments.ValueObjects.PaymentId(command.PaymentId),
                cancellationToken);

        if (payment is null)
            return Result.Failure(PaymentErrors.NotFound);

        var result = payment.Refund();

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}