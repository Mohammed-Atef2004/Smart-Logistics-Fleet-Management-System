using Application.Features.Users;
using Application.Features.Users.Services;

namespace Application.Tests.Fakes;

public sealed class FakeAuditService : IAuditService
{
    public List<AuditEntry> Entries { get; } = [];

    public Task LogAsync(AuditEntry entry, CancellationToken ct = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}
