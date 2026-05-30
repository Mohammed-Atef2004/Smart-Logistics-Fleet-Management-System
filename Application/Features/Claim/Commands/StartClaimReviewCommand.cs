using Domain.Claims;
using Domain.Claims.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Claims.Commands.StartClaimReview;

public record StartClaimReviewCommand(Guid ClaimId) : IRequest<Unit>;

public class StartClaimReviewCommandHandler : IRequestHandler<StartClaimReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;

    public StartClaimReviewCommandHandler(IUnitOfWork unitOfWork, IClaimRepository claimRepository)
    {
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
    }

    public async Task<Unit> Handle(StartClaimReviewCommand request, CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.EntityQuery.FirstOrDefaultAsync(x=>x.Id==new ClaimId(request.ClaimId), cancellationToken);

        if (claim is null)
            throw new Exception($"Claim '{request.ClaimId}' was not found.");

        var result = claim.StartReview();

        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        await _unitOfWork.CompleteAsync();
        return Unit.Value;
    }
}
