namespace Application.Features.Users.Commands.Authentication.Logout
{
    public sealed record LogoutCommand() : IRequest<Result>;
}