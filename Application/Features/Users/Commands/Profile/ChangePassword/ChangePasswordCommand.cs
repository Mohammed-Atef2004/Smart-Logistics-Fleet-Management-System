namespace Application.Features.Users.Commands.Profile.ChangePassword
{
    public record ChangePasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword,
        string? TwoFactorCode) : IRequest<Result>;
}