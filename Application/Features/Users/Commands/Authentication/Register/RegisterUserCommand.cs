namespace Application.Features.Users.Commands.Authentication.Register
{
    public sealed record RegisterUserCommand(
    string Email,
    string Username,
    string Password,
    string FirstName,
    string LastName) : IRequest<Result<Guid>>;
}
