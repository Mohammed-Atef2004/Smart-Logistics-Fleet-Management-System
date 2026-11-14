namespace Application.Features.Users.Queries.GetUserById
{
    public record GetUserByIdResponse(
     Guid Id,
     string Username,
     string Email,
     string firstname,
     string lastname,
     string? phoneNumber,
     string Role,
     bool IsActive,
     bool IsEmailConfirmed,
     DateTime? LastLogin);
}