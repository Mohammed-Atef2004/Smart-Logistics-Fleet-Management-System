using Domain.Claims.Enums;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

internal class ClaimMustBeSubmittedRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeSubmittedRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim must be in Submitted status to start review.");

    public bool IsBroken() => _status != ClaimStatus.Submitted;
}
