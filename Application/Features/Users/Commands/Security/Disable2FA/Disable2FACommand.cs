namespace Application.Features.Users.Commands.Security.Disable2FA
{
    public sealed record Disable2FACommand(Guid UserId) : IRequest<Result>;

}