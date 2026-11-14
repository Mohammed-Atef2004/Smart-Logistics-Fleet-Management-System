namespace Application.Features.Users.Commands.Admin.UnlockAccount
{
    public sealed record UnlockAccountCommand(Guid UserId, Guid ActorId) : IRequest<Result>;
}