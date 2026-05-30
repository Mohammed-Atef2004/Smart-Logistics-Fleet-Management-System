using Domain.Claims;
using Domain.Claims.ValueObjects;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Claims.Commands.SubmitClaim;

public record SubmitClaimCommand(
    Guid ShipmentId,
    Guid CustomerId,
    decimal Amount,
    string Currency,
    string Description,
    string? DocumentFileName,
    string? DocumentFileUrl,
    string? DocumentContentType,
    long? DocumentFileSizeBytes
) : IRequest<Guid>;

public class SubmitClaimCommandHandler
    : IRequestHandler<SubmitClaimCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;

    public SubmitClaimCommandHandler(
        IUnitOfWork unitOfWork,
        IClaimRepository claimRepository)
    {
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
    }

    public async Task<Guid> Handle(
        SubmitClaimCommand request,
        CancellationToken cancellationToken)
    {
        var amount = ClaimAmount.Of(
            request.Amount,
            request.Currency);

        ClaimDocument? document = null;

        if (request.DocumentFileName is not null &&
            request.DocumentFileUrl is not null &&
            request.DocumentContentType is not null &&
            request.DocumentFileSizeBytes is not null)
        {
            document = ClaimDocument.Create(
                request.DocumentFileName,
                request.DocumentFileUrl,
                request.DocumentContentType,
                request.DocumentFileSizeBytes.Value);
        }

        var result = InsuranceClaim.Submit(
            request.ShipmentId,
            request.CustomerId,
            amount,
            request.Description,
            document);

        if (result.IsFailure)
            throw new Exception(result.Error.Message);

        await _claimRepository.AddAsync(
            result.Value);

        await _unitOfWork.CompleteAsync();

        return result.Value.Id.Value;
    }
}