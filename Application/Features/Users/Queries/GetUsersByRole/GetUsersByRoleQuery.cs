using Application.Features.Users.Queries.GetUserById;

namespace Application.Features.Users.Queries.GetUsersByRole
{
    public record GetUsersByRoleQuery(string Role) : IRequest<Result<List<GetUsersByRoleResponse>>>;
}