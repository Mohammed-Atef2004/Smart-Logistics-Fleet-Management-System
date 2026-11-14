namespace Application.Features.Users.Commands.Authentication.VerifyTwoFactorLogin
{
    public sealed class VerifyTwoFactorLoginValidator : AbstractValidator<VerifyTwoFactorLoginCommand>
    {
        public VerifyTwoFactorLoginValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.TotpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be exactly 6 digits.")
                .Matches(@"^[0-9]*$").WithMessage("OTP code must contain only numbers.");
        }
    }
}