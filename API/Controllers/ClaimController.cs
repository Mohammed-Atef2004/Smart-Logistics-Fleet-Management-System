using Application.Features.Claims.Commands.ApproveClaim;
using Application.Features.Claims.Commands.ProcessClaimPayment;
using Application.Features.Claims.Commands.RejectClaim;
using Application.Features.Claims.Commands.StartClaimReview;
using Application.Features.Claims.Commands.SubmitClaim;
using Application.Features.Claims.DTOs;
using Application.Features.Claims.Queries.GetClaimDetails;
using Application.Features.Claims.Queries.GetClaimsByStatus;
using Application.Features.Claims.Queries.GetPendingClaims;
using Domain.Claims.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SLFMS.API.Controllers;

[ApiController]
[Route("api/claims")]
public class ClaimsController : ControllerBase
{
    private readonly ISender _sender;

    public ClaimsController(ISender sender)
    {
        _sender = sender;
    }

    // =========================================================
    // SUBMIT CLAIM
    // =========================================================
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitClaim(
        SubmitClaimCommand command,
        CancellationToken ct)
    {
        var claimId = await _sender.Send(command, ct);

        return Created($"/api/claims/{claimId}", new { id = claimId });
    }

    // =========================================================
    // START REVIEW
    // =========================================================
    [HttpPut("{id:guid}/start-review")]
    public async Task<IActionResult> StartReview(
        Guid id,
        CancellationToken ct)
    {
        await _sender.Send(new StartClaimReviewCommand(id), ct);
        return NoContent();
    }

    // =========================================================
    // APPROVE CLAIM
    // =========================================================
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        ApproveClaimRequest request,
        CancellationToken ct)
    {
        await _sender.Send(
            new ApproveClaimCommand(id, request.ApprovedAmount, request.Currency),
            ct);

        return NoContent();
    }

    // =========================================================
    // REJECT CLAIM
    // =========================================================
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        RejectClaimRequest request,
        CancellationToken ct)
    {
        await _sender.Send(
            new RejectClaimCommand(id, request.Reason),
            ct);

        return NoContent();
    }

    // =========================================================
    // PROCESS PAYMENT
    // =========================================================
    [HttpPut("{id:guid}/process-payment")]
    public async Task<IActionResult> ProcessPayment(
        Guid id,
        CancellationToken ct)
    {
        await _sender.Send(new ProcessClaimPaymentCommand(id), ct);
        return NoContent();
    }

    // =========================================================
    // GET CLAIM DETAILS
    // =========================================================
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetClaimDetailsQuery(id), ct);

        return result is null
            ? NotFound()
            : Ok(result);
    }

    // =========================================================
    // GET PENDING CLAIMS
    // =========================================================
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetPendingClaimsQuery(), ct);
        return Ok(result);
    }

    // =========================================================
    // GET BY STATUS
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> GetByStatus(
        [FromQuery] ClaimStatus status,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetClaimsByStatusQuery(status), ct);
        return Ok(result);
    }
}

// =========================================================
// Request DTOs
// =========================================================

public sealed record ApproveClaimRequest(
    decimal ApprovedAmount,
    string Currency);

public sealed record RejectClaimRequest(
    string Reason);