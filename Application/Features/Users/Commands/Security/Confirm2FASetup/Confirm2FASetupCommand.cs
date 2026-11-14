namespace Application.Features.Users.Commands.Security.Confirm2FASetup
{
    public record Confirm2FASetupCommand(
           Guid UserId,
           string TotpCode,
           string NewSecret
       ) : IRequest<Result>;
}