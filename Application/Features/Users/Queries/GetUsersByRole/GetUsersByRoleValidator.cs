using Domain.Users;

namespace Application.Features.Users.Queries.GetUsersByRole
{
    public class GetUsersByRoleValidator : AbstractValidator<GetUsersByRoleQuery>
    {
        public GetUsersByRoleValidator()
        {
            RuleFor(x => x.Role)
                .IsEnumName(typeof(UserRole), caseSensitive: false)
                .WithMessage("The provided role is not a valid system role.");
        }
    }
}