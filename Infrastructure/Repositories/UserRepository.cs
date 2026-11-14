using Application.Common.Interfaces;
using Domain.Users;
using Domain.Users.ValueObjects;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
            public UserRepository(AppDbContext context) : base(context)
        {
            }

        // Compare the value object directly (u.Email == email)
        // EF uses the configured value converter automatically
        public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default)
            => _context.Users
                .AnyAsync(u => u.Email == email, ct);

        public Task<bool> ExistsByUsernameAsync(Username username, CancellationToken ct = default)
            => _context.Users
                .AnyAsync(u => u.Username == username, ct);

        public Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
            => _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        public Task<User?>GetByIdAsync(Guid id, CancellationToken ct = default)
            =>_context.Users.
            FirstOrDefaultAsync(u=>u.Id == id, ct);
        public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct = default)
            => _context.Users
                .FirstOrDefaultAsync(u => u.Username == username, ct);
    }
}
