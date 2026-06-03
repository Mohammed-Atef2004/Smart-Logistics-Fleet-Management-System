using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimReviewStartedEvent(
    ClaimId id) : DomainEvent;
