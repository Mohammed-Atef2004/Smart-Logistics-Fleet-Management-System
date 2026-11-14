using Application.Features.Users.Queries.GetUserById;

namespace Application.Features.Users.Queries.GetUserByEmail
{
    public record GetUserByEmailQuery(string Email) : IRequest<Result<GetUserByEmailResponse>>;
}