namespace Application.Features.Users
{
    public sealed record AuditEntry(
     Guid ActorId,        
     string Action,          
     string EntityType,       
     Guid EntityId,         
     string? IpAddress,        
     bool Succeeded,
     string? FailureReason = null,
     DateTime? OccurredAt = null)
    {
        public DateTime OccurredAtUtc => OccurredAt ?? DateTime.UtcNow;
    }
}