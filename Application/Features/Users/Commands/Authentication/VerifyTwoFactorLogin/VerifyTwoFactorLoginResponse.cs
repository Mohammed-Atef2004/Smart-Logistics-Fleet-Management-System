namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactorLogin
{
    public record VerifyTwoFactorLoginResponse(string Token, string RefreshToken, Guid UserId);
}