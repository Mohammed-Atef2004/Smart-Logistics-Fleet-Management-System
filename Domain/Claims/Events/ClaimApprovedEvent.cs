using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims.Events;

public record ClaimApprovedEvent(
    ClaimId id,
    Guid CustomerId,
    ClaimAmount ApprovedAmount) : DomainEvent;
