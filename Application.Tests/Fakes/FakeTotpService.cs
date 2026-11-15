using Application.Features.Users.Services;

namespace Application.Tests.Fakes;

/// <summary>
/// Deterministic ITotpService: the test decides which secret/code pairs
/// are valid via SetValid.
/// </summary>
public sealed class FakeTotpService : ITotpService
{
    private readonly HashSet<(string Secret, string Code)> _validPairs = [];

    public void SetValid(string secret, string code) => _validPairs.Add((secret, code));

    public string GenerateSecret() => "TESTSECRET32BASE";

    public string GenerateQrCodeUri(string email, string secret)
        => $"otpauth://totp/SLFMS:{email}?secret={secret}";

    public bool Verify(string secret, string code)
        => !string.IsNullOrEmpty(secret) && _validPairs.Contains((secret, code));
}
