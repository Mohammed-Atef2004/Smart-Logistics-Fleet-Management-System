namespace Application.Features.Users.Commands.Profile.ChangeEmail
{
    public sealed record ChangeEmailCommand(
        Guid UserId,
        string NewEmail,
        string CurrentPassword,
        string? TwoFactorCode) : IRequest<Result>;
}