namespace Application.Features.Users.Commands.Admin.Deactivate
{
    public sealed record DeactivateUserCommand(
     Guid UserId,
     Guid ActorId,
     string Reason
 ) : IRequest<Result>;
}