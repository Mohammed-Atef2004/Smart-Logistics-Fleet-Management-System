using Application.Features.Claims.DTOs;
using Domain.Claims;
using Domain.Claims.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Claims.Queries.GetClaimDetails;

public record GetClaimDetailsQuery(Guid ClaimId) : IRequest<ClaimDto>;

public class GetClaimDetailsQueryHandler : IRequestHandler<GetClaimDetailsQuery, ClaimDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;

    public GetClaimDetailsQueryHandler(IUnitOfWork unitOfWork, IClaimRepository claimRepository)
    {
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
    }
    public async Task<ClaimDto> Handle(GetClaimDetailsQuery request, CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.EntityQuery.FirstOrDefaultAsync(x => x.Id == new ClaimId(request.ClaimId), cancellationToken);

        if (claim is null)
            throw new Exception($"Claim '{request.ClaimId}' was not found.");

        return new ClaimDto(
            Id: claim.Id.Value,
            ClaimNumber: claim.ClaimNumber.Value,
            ShipmentId: claim.ShipmentId,
            CustomerId: claim.CustomerId,
            Status: claim.Status.ToString(),
            ClaimAmount: claim.ClaimAmount.Value,
            ClaimCurrency: claim.ClaimAmount.Currency,
            ApprovedAmount: claim.ApprovedAmount?.Value,
            ApprovedCurrency: claim.ApprovedAmount?.Currency,
            Description: claim.Description,
            RejectionReason: claim.RejectionReason,
            SubmittedAt: claim.SubmittedAt,
            ReviewedAt: claim.ReviewedAt,
            ProcessedAt: claim.ProcessedAt,
            Document: claim.SupportingDocument is null ? null : new ClaimDocumentDto(
                claim.SupportingDocument.FileName,
                claim.SupportingDocument.FileUrl,
                claim.SupportingDocument.ContentType,
                claim.SupportingDocument.FileSizeBytes,
                claim.SupportingDocument.UploadedAt),
            Items: claim.Items.Select(i => new ClaimItemDto(
                i.Id,
                i.Description,
                i.UnitValue.Value,
                i.UnitValue.Currency,
                i.Quantity,
                i.TotalValue.Value)).ToList()
        );
    }
}
