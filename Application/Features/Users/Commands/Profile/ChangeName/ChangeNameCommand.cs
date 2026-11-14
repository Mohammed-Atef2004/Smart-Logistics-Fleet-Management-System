namespace Application.Features.Users.Commands.Profile
{
    public sealed record ChangeNameCommand(
        Guid UserId,    
        string FirstName, 
        string LastName
    ) : IRequest<Result>;
}