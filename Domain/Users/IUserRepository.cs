using Domain.Users.ValueObjects;

namespace Domain.Users;

/// <summary>
/// Repository contract for the User aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct = default);

    Task AddAsync(User entity);
    void Update(User entity);
    void Delete(User entity);
}
