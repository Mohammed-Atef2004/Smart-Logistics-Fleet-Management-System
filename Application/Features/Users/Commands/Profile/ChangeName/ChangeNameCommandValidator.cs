namespace Application.Features.Users.Commands.Profile
{
    public sealed class ChangeNameCommandValidator : AbstractValidator<ChangeNameCommand>
    {
        public ChangeNameCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
        }
    }
}