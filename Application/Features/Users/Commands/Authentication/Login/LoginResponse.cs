namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactor
{
    public sealed record LoginResponse(
    string? Token,
    string? RefreshToken, 
    Guid UserId,
    bool RequiresTwoFactor
);
}
