namespace Application.Features.Users.Commands.Security.Disable2FA
{
    public sealed class Disable2FACommandValidator : AbstractValidator<Disable2FACommand>
    {
        public Disable2FACommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }

}