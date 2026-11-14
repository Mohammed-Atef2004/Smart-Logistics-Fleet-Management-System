namespace Application.Features.Users.Commands.Admin.Delete
{
    public sealed record DeleteUserCommand(
    Guid UserId,
    Guid ActorId,
    string Reason
) : IRequest<Result>;
}