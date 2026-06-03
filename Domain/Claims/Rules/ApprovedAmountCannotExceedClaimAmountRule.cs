using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

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
