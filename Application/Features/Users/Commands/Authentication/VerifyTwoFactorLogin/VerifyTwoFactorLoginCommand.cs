namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactorLogin
{
    public record VerifyTwoFactorLoginCommand(Guid UserId, string TotpCode)
          : IRequest<Result<VerifyTwoFactorLoginResponse>>;

}