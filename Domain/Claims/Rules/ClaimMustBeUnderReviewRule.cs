using Domain.Claims.Enums;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

internal class ClaimMustBeUnderReviewRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeUnderReviewRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim must be Under Review to approve or reject.");

    public bool IsBroken() => _status != ClaimStatus.UnderReview;
}
