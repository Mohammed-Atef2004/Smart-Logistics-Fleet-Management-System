using Domain.Claims;
using Domain.Claims.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Claims.Commands.RejectClaim;

public record RejectClaimCommand(
    Guid ClaimId,
    string Reason
) : IRequest<Unit>;

public class RejectClaimCommandHandler
    : IRequestHandler<RejectClaimCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;

    public RejectClaimCommandHandler(
        IUnitOfWork unitOfWork,
        IClaimRepository claimRepository)
    {
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
    }

    public async Task<Unit> Handle(
        RejectClaimCommand request,
        CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.EntityQuery
            .FirstOrDefaultAsync(
                x => x.Id == new ClaimId(request.ClaimId),
                cancellationToken);

        if (claim is null)
            throw new Exception(
                $"Claim '{request.ClaimId}' was not found.");

        var result = claim.Reject(request.Reason);

        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        await _unitOfWork.CompleteAsync();

        return Unit.Value;
    }
}