namespace Application.Features.Users.Commands.Authentication.ConfirmEmail
{
    public sealed record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<Result>;
}