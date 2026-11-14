namespace Application.Features.Users.Commands.Profile.SetPhoneNumber
{
    public record SetPhoneNumberCommand(
        Guid UserId,
        string NewPhoneNumber,
        string CurrentPassword,
        string? TwoFactorCode) : IRequest<Result>;
}