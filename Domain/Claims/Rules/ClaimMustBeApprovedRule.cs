using Domain.Claims.Enums;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

internal class ClaimMustBeApprovedRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimMustBeApprovedRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Claim must be Approved before processing payment.");

    public bool IsBroken() => _status != ClaimStatus.Approved;
}
