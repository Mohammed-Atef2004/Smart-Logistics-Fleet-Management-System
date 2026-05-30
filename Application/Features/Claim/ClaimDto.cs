using Domain.Claims.Enums;

namespace Application.Features.Claims.DTOs;

public record ClaimDto(
    Guid Id,
    string ClaimNumber,
    Guid ShipmentId,
    Guid CustomerId,
    string Status,
    decimal ClaimAmount,
    string ClaimCurrency,
    decimal? ApprovedAmount,
    string? ApprovedCurrency,
    string Description,
    string? RejectionReason,
    DateTime SubmittedAt,
    DateTime? ReviewedAt,
    DateTime? ProcessedAt,
    ClaimDocumentDto? Document,
    List<ClaimItemDto> Items
);

public record ClaimSummaryDto(
    Guid Id,
    string ClaimNumber,
    Guid ShipmentId,
    Guid CustomerId,
    string Status,
    decimal ClaimAmount,
    string Currency,
    DateTime SubmittedAt
);

public record ClaimDocumentDto(
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSizeBytes,
    DateTime UploadedAt
);

public record ClaimItemDto(
    Guid Id,
    string Description,
    decimal UnitValue,
    string Currency,
    int Quantity,
    decimal TotalValue
);
