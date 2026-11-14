namespace Application.Features.Users.Queries.GetUsersByRole
{
    public record GetUsersByRoleResponse(
        Guid Id,
        string Username,
        string Email,
        bool IsActive);
}