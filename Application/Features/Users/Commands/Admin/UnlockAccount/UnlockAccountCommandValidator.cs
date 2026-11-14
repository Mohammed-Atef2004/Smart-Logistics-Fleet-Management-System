namespace Application.Features.Users.Commands.Admin.UnlockAccount
{
    public sealed class UnlockAccountCommandValidator : AbstractValidator<UnlockAccountCommand>
    {
        public UnlockAccountCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.ActorId)
                .NotEmpty().WithMessage("Actor ID is required.");
        }
    }
}