using Domain.Users;
using Domain.Users.ValueObjects;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory IUserRepository for handler tests. Users are tracked so the
/// tests can assert on aggregate state transitions after a handler runs.
/// </summary>
public sealed class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = [];

    /// <summary>Entities passed to Update(), so tests can check it was called.</summary>
    public List<User> Updated { get; } = [];

    /// <summary>Entities passed to AddAsync().</summary>
    public List<User> Added { get; } = [];

    public FakeUserRepository(params User[] seed)
    {
        foreach (var user in seed)
            _users[user.Id] = user;
    }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        => Task.FromResult(_users.Values.FirstOrDefault(u => u.Email.Value == email.Value));

    public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default)
        => Task.FromResult(_users.Values.FirstOrDefault(u => u.Username.Value == username.Value));

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_users.GetValueOrDefault(id));

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default)
        => Task.FromResult(_users.Values.Any(u => u.Email.Value == email.Value));

    public Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct = default)
        => Task.FromResult(_users.Values.Any(u => u.Username.Value == username.Value));

    public Task AddAsync(User entity)
    {
        _users[entity.Id] = entity;
        Added.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(User entity) => Updated.Add(entity);

    public void Delete(User entity) => _users.Remove(entity.Id);
}
