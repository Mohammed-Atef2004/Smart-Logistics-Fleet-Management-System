using Application.Common.Interfaces;

namespace Application.Tests.Fakes;

/// <summary>
/// Deterministic IPasswordHasher for tests: "hash" is just a prefixed
/// value so assertions can compare hashes without BCrypt.
/// </summary>
public sealed class FakePasswordHasher : IPasswordHasher
{
    public const string Prefix = "hashed:";

    public string HashPassword(string password) => Prefix + password;

    public bool VerifyPassword(string password, string hashedPassword)
        => hashedPassword == Prefix + password;
}
