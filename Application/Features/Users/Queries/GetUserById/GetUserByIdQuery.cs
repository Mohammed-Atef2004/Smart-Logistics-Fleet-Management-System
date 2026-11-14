namespace Application.Features.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<Result<GetUserByIdResponse>>;
}