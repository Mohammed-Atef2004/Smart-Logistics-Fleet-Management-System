using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimItemAddedEvent(
    ClaimId id,
    string ItemDescription) : DomainEvent;
