namespace Application.Features.Users.Queries.GetUserByEmail
{
    public record GetUserByEmailResponse(
        Guid Id,
        string Username,
        string Email,
        string Role,
        bool IsActive,
        DateTime LastLogin);
}