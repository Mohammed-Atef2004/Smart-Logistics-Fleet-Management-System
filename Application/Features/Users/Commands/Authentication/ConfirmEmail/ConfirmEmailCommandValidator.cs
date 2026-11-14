namespace Application.Features.Users.Commands.Authentication.ConfirmEmail
{
    public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Confirmation token is required.");
        }
    }
}