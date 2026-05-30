using Domain.Claims.Enums;
using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

internal class ClaimMustBeSubmittedRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeSubmittedRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim must be in Submitted status to start review.");

    public bool IsBroken() => _status != ClaimStatus.Submitted;
}

internal class ClaimMustBeUnderReviewRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeUnderReviewRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim must be Under Review to approve or reject.");

    public bool IsBroken() => _status != ClaimStatus.UnderReview;
}

internal class ClaimMustBeApprovedRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeApprovedRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim must be Approved before processing payment.");

    public bool IsBroken() => _status != ClaimStatus.Approved;
}

internal class ClaimMustBeEditableRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeEditableRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim can only be edited when Submitted or Under Review.");

    public bool IsBroken() => _status is not (ClaimStatus.Submitted or ClaimStatus.UnderReview);
}

internal class ClaimCanBeClosedRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimCanBeClosedRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Only Rejected or Payment Processed claims can be closed.");

    public bool IsBroken() => _status is not (ClaimStatus.Rejected or ClaimStatus.PaymentProcessed);
}

internal class ApprovedAmountCannotExceedClaimAmountRule : IBusinessRule
{
    private readonly ClaimAmount _approved;
    private readonly ClaimAmount _claimed;

    public ApprovedAmountCannotExceedClaimAmountRule(ClaimAmount approved, ClaimAmount claimed)
    {
        _approved = approved;
        _claimed = claimed;
    }

    public Error Error => new("Claim.ApprovedAmount", $"Approved amount ({_approved}) cannot exceed claimed amount ({_claimed}).");

    public bool IsBroken() => _approved.Value > _claimed.Value;
}
