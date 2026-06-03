using Domain.Claims.Enums;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

internal class ClaimMustBeEditableRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeEditableRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim can only be edited when Submitted or Under Review.");

    public bool IsBroken() => _status is not (ClaimStatus.Submitted or ClaimStatus.UnderReview);
}
