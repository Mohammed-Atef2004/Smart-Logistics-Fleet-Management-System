namespace Application.Features.Payments
{
    public record PaymentDetailsDto(
        Guid Id,
        Guid InvoiceId,
        decimal Amount,
        string Status,
        string PaymentMethodType,
        string? Provider,
        string? Last4Digits,
        string? TransactionReference,
        DateTime? ProcessedAt,
        string? FailureReason
    );

    public record PaymentListItemDto(
        Guid Id,
        Guid InvoiceId,
        decimal Amount,
        string Status,
        string PaymentMethodType,
        DateTime? ProcessedAt
    );
}
