using Domain.Claims.Enums;
using Domain.Claims.Errors;
using Domain.Claims.Events;
using Domain.Claims.Rules;
using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims;

public sealed class InsuranceClaim : AggregateRoot<ClaimId>
{
    private readonly List<ClaimItem> _items = new();
    public IReadOnlyList<ClaimItem> Items => _items.AsReadOnly();

    public ClaimNumber ClaimNumber { get; private set; }
    public Guid ShipmentId { get; private set; }
    public Guid CustomerId { get; private set; }
    public ClaimStatus Status { get; private set; }
    public ClaimAmount ClaimAmount { get; private set; }
    public ClaimAmount? ApprovedAmount { get; private set; }
    public string Description { get; private set; }
    public ClaimDocument? SupportingDocument { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private InsuranceClaim() { } // EF Core

    private InsuranceClaim(ClaimId id, Guid shipmentId, Guid customerId, ClaimAmount amount, string description)
        : base(id)
    {
        ClaimNumber = ClaimNumber.Generate();
        ShipmentId = shipmentId;
        CustomerId = customerId;
        ClaimAmount = amount;
        Description = description;
        Status = ClaimStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    // ==================== Factory ====================

    public static Result<InsuranceClaim> Submit(
        Guid shipmentId,
        Guid customerId,
        ClaimAmount amount,
        string description,
        ClaimDocument? document = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result<InsuranceClaim>.Failure(ClaimErrors.EmptyDescription);

        if (!amount.IsPositive())
            return Result<InsuranceClaim>.Failure(ClaimErrors.InvalidAmount);

        if (document is not null && document.ExceedsMaxSize())
            return Result<InsuranceClaim>.Failure(ClaimErrors.DocumentTooLarge);

        var claim = new InsuranceClaim(new ClaimId(Guid.NewGuid()), shipmentId, customerId, amount, description);
        claim.SupportingDocument = document;
        claim.AddDomainEvent(new ClaimSubmittedEvent(claim.Id, shipmentId, customerId, amount));

        return Result<InsuranceClaim>.Success(claim);
    }

    // ==================== Business Methods ====================

    public Result AddItem(string description, ClaimAmount unitValue, int quantity)
    {
        CheckRule(new ClaimMustBeEditableRule(Status));

        var itemResult = ClaimItem.Create(description, unitValue, quantity);
        if (itemResult.IsFailure)
            return Result.Failure(itemResult.Error);

        _items.Add(itemResult.Value);
        AddDomainEvent(new ClaimItemAddedEvent(Id, description));

        return Result.Success();
    }

    public Result AttachDocument(ClaimDocument document)
    {
        CheckRule(new ClaimMustBeEditableRule(Status));

        if (document.ExceedsMaxSize())
            return Result.Failure(ClaimErrors.DocumentTooLarge);

        SupportingDocument = document;
        return Result.Success();
    }

    public Result StartReview()
    {
        CheckRule(new ClaimMustBeSubmittedRule(Status));

        Status = ClaimStatus.UnderReview;
        ReviewedAt = DateTime.UtcNow;
        AddDomainEvent(new ClaimReviewStartedEvent(Id));

        return Result.Success();
    }

    public Result Approve(ClaimAmount approvedAmount)
    {
        CheckRule(new ClaimMustBeUnderReviewRule(Status));
        CheckRule(new ApprovedAmountCannotExceedClaimAmountRule(approvedAmount, ClaimAmount));

        if (!approvedAmount.IsPositive())
            return Result.Failure(ClaimErrors.InvalidAmount);

        ApprovedAmount = approvedAmount;
        Status = ClaimStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
        AddDomainEvent(new ClaimApprovedEvent(Id, CustomerId, approvedAmount));

        return Result.Success();
    }

    public Result Reject(string reason)
    {
        CheckRule(new ClaimMustBeUnderReviewRule(Status));

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(ClaimErrors.EmptyRejectionReason);

        RejectionReason = reason;
        Status = ClaimStatus.Rejected;
        ReviewedAt = DateTime.UtcNow;
        AddDomainEvent(new ClaimRejectedEvent(Id, CustomerId, reason));

        return Result.Success();
    }

    public Result ProcessPayment()
    {
        CheckRule(new ClaimMustBeApprovedRule(Status));

        Status = ClaimStatus.PaymentProcessed;
        ProcessedAt = DateTime.UtcNow;
        AddDomainEvent(new ClaimPaymentProcessedEvent(Id, CustomerId, ApprovedAmount!));

        return Result.Success();
    }

    public Result Close()
    {
        CheckRule(new ClaimCanBeClosedRule(Status));

        Status = ClaimStatus.Closed;
        AddDomainEvent(new ClaimClosedEvent(Id));

        return Result.Success();
    }

    // ==================== Helpers ====================

    public bool IsPending() =>
        Status is ClaimStatus.Submitted or ClaimStatus.UnderReview;

    public bool IsResolved() =>
        Status is ClaimStatus.Rejected or ClaimStatus.PaymentProcessed or ClaimStatus.Closed;
}
