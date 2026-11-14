namespace Application.Features.Users.Commands.Admin.ReActivate
{
    public sealed record ReactivateUserCommand(Guid UserId, Guid ActorId) : IRequest<Result>;
}