using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimSubmittedEvent(
    ClaimId id,
    Guid ShipmentId,
    Guid CustomerId,
    ClaimAmount Amount) : DomainEvent;

public record ClaimReviewStartedEvent(
    ClaimId id) : DomainEvent;

public record ClaimApprovedEvent(
    ClaimId id,
    Guid CustomerId,
    ClaimAmount ApprovedAmount) : DomainEvent;

public record ClaimRejectedEvent(
    ClaimId id,
    Guid CustomerId,
    string Reason) : DomainEvent;

public record ClaimPaymentProcessedEvent(
    ClaimId id,
    Guid CustomerId,
    ClaimAmount PaidAmount) : DomainEvent;

public record ClaimClosedEvent(
    ClaimId id) : DomainEvent;

public record ClaimItemAddedEvent(
    ClaimId id,
    string ItemDescription) : DomainEvent;
