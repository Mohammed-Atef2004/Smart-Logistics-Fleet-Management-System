namespace Application.Features.Users.Services
{
    public interface IAuditService
    {
        Task LogAsync(AuditEntry entry, CancellationToken ct = default);
    }
}