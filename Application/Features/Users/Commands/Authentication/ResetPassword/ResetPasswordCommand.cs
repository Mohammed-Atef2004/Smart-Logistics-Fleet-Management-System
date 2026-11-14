namespace Application.Features.Users.Commands.Authentication.ResetPassword
{
    public record ResetPasswordCommand(
        Guid UserId,
        string Token,
        string NewPassword,
        string? TwoFactorCode) : IRequest<Result>;
}