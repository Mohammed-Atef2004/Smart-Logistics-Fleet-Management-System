using Application.Features.Claims.DTOs;
using Domain.Claims;
using Domain.Claims.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Claims.Queries.GetClaimsByStatus;

public record GetClaimsByStatusQuery(ClaimStatus Status)
    : IRequest<List<ClaimSummaryDto>>;

public class GetClaimsByStatusQueryHandler
    : IRequestHandler<GetClaimsByStatusQuery, List<ClaimSummaryDto>>
{
    private readonly IClaimRepository _claimRepository;

    public GetClaimsByStatusQueryHandler(
        IClaimRepository claimRepository)
    {
        _claimRepository = claimRepository;
    }

    public async Task<List<ClaimSummaryDto>> Handle(
        GetClaimsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var claims = await _claimRepository.EntityQuery
            .Where(x => x.Status == request.Status)
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