using Domain.Claims.Enums;
using Domain.SharedKernel;

namespace Domain.Claims.Rules;

internal class ClaimCanBeClosedRule : IBusinessRule
{
    private readonly ClaimStatus _status;

    public ClaimCanBeClosedRule(ClaimStatus status) => _status = status;

    public Error Error => new("Claim.Status", "Only Rejected or Payment Processed claims can be closed.");

    public bool IsBroken() => _status is not (ClaimStatus.Rejected or ClaimStatus.PaymentProcessed);
}
