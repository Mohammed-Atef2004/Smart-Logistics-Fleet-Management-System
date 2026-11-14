namespace Application.Features.Users.Commands.Authentication.ConfirmEmailChange
{
    public sealed record ConfirmEmailChangeCommand(
        Guid UserId,
        string NewEmail,
        string Token) : IRequest<Result<string>>;
}