namespace Application.Features.Users.Commands.Authentication.ConfirmEmailChange
{
    public sealed class ConfirmEmailChangeCommandValidator : AbstractValidator<ConfirmEmailChangeCommand>
    {
        public ConfirmEmailChangeCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("New email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required to confirm email change.");
        }
    }
}