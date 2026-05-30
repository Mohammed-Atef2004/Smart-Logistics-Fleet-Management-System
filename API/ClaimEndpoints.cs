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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SLFMS.API.Endpoints;

public static class ClaimEndpoints
{
    public static void MapClaimEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/claims")
            .WithTags("Insurance Claims")
            .RequireAuthorization();

        // =========================================================
        // COMMANDS
        // =========================================================

        // POST /api/claims/submit
        group.MapPost("/submit", async (
            SubmitClaimCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            // SubmitClaim returns Guid
            var claimId = await sender.Send(command, ct);

            return Results.Created(
                $"/api/claims/{claimId}",
                new { id = claimId });
        })
        .WithName("SubmitClaim")
        .Produces<object>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Submit a new insurance claim for a shipment");

        // PUT /api/claims/{id}/start-review
        group.MapPut("/{id:guid}/start-review", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            // StartReview returns Unit
            await sender.Send(new StartClaimReviewCommand(id), ct);

            return Results.NoContent();
        })
        .WithName("StartClaimReview")
        .Produces(StatusCodes.Status204NoContent)
        .WithSummary("Move claim to Under Review status");

        // PUT /api/claims/{id}/approve
        group.MapPut("/{id:guid}/approve", async (
            Guid id,
            ApproveClaimRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ApproveClaimCommand(
                id,
                request.ApprovedAmount,
                request.Currency);

            // Approve returns Unit
            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .WithName("ApproveClaim")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Approve a claim with a specified approved amount");

        // PUT /api/claims/{id}/reject
        group.MapPut("/{id:guid}/reject", async (
            Guid id,
            RejectClaimRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RejectClaimCommand(id, request.Reason);

            // Reject returns Unit
            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .WithName("RejectClaim")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Reject a claim with a specified reason");

        // PUT /api/claims/{id}/process-payment
        group.MapPut("/{id:guid}/process-payment", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            // ProcessPayment returns Unit
            await sender.Send(new ProcessClaimPaymentCommand(id), ct);

            return Results.NoContent();
        })
        .WithName("ProcessClaimPayment")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Process payment for an approved claim");

        // =========================================================
        // QUERIES
        // =========================================================

        // GET /api/claims/{id}
        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetClaimDetailsQuery(id), ct);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetClaimDetails")
        .Produces<ClaimDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get full details of a specific claim");

        // GET /api/claims/pending
        group.MapGet("/pending", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetPendingClaimsQuery(), ct);

            return Results.Ok(result);
        })
        .WithName("GetPendingClaims")
        .Produces<IReadOnlyList<ClaimSummaryDto>>(StatusCodes.Status200OK)
        .WithSummary("Get all pending claims (Submitted + Under Review)");

        // GET /api/claims?status=Approved
        group.MapGet("/", async (
            ClaimStatus status,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetClaimsByStatusQuery(status), ct);

            return Results.Ok(result);
        })
        .WithName("GetClaimsByStatus")
        .Produces<IReadOnlyList<ClaimSummaryDto>>(StatusCodes.Status200OK)
        .WithSummary("Get claims filtered by status");
    }
}

// =========================================================
// Request Models (thin DTOs for API input)
// =========================================================

public sealed record ApproveClaimRequest(
    decimal ApprovedAmount,
    string Currency);

public sealed record RejectClaimRequest(
    string Reason);