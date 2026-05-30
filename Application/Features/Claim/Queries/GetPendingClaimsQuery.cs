using Application.Features.Claims.DTOs;
using Domain.Claims;
using Domain.Claims.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Claims.Queries.GetPendingClaims;

public record GetPendingClaimsQuery
    : IRequest<List<ClaimSummaryDto>>;

public class GetPendingClaimsQueryHandler
    : IRequestHandler<GetPendingClaimsQuery, List<ClaimSummaryDto>>
{
    private readonly IClaimRepository _claimRepository;

    public GetPendingClaimsQueryHandler(
        IClaimRepository claimRepository)
    {
        _claimRepository = claimRepository;
    }

    public async Task<List<ClaimSummaryDto>> Handle(
        GetPendingClaimsQuery request,
        CancellationToken cancellationToken)
    {
        var claims = await _claimRepository.EntityQuery
            .Where(c =>
                c.Status == ClaimStatus.Submitted ||
                c.Status == ClaimStatus.UnderReview)
            .OrderBy(c => c.SubmittedAt)
            .ToListAsync(cancellationToken);

        return claims
            .Select(c => new ClaimSummaryDto(
                c.Id.Value,
                c.ClaimNumber.Value,
                c.ShipmentId,
                c.CustomerId,
                c.Status.ToString(),
                c.ClaimAmount.Value,
                c.ClaimAmount.Currency,
                c.SubmittedAt))
            .ToList();
    }
}