using Domain.Claims;
using Domain.Claims.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Claims.Commands.ApproveClaim;

public record ApproveClaimCommand(
    Guid ClaimId,
    decimal ApprovedAmount,
    string Currency
) : IRequest<Unit>;

public class ApproveClaimCommandHandler : IRequestHandler<ApproveClaimCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;

    public ApproveClaimCommandHandler(
        IUnitOfWork unitOfWork,
        IClaimRepository claimRepository)
    {
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
    }

    public async Task<Unit> Handle(
        ApproveClaimCommand request,
        CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.EntityQuery
            .FirstOrDefaultAsync(
                x => x.Id == new ClaimId(request.ClaimId),
                cancellationToken);

        if (claim is null)
            throw new Exception(
                $"Claim '{request.ClaimId}' was not found.");

        var approvedAmount = ClaimAmount.Of(
            request.ApprovedAmount,
            request.Currency);

        var result = claim.Approve(approvedAmount);

        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}