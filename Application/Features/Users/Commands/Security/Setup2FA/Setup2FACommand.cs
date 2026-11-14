namespace Application.Features.Users.Commands.Security.Setup2FA
{
    public record Setup2FACommand(Guid UserId) : IRequest<Result<Setup2FAResponse>>;
}