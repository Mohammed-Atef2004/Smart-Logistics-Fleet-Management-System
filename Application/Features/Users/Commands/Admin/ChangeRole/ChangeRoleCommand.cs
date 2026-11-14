using Domain.Users;

namespace Application.Features.Users.Commands.Admin.ChangeRole
{
    public sealed record ChangeRoleCommand(
    Guid TargetUserId,
    Guid ActorId,
    UserRole NewRole
) : IRequest<Result>;
}