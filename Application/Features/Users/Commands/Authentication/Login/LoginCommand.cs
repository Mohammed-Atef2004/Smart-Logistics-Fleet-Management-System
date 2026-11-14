using Application.Features.Users.Commands.Authentication.VerifyTwoFactor;

namespace Application.Features.Users.Commands.Authentication.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password) : IRequest<Result<LoginResponse>>;
}