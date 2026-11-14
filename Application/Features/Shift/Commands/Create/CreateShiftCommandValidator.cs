namespace Application.Features.Shift.Commands.Create
{
    public class CreateShiftCommandValidator:AbstractValidator<CreateShiftCommand>
    {
        public CreateShiftCommandValidator()
        {
            RuleFor(x => x.driverId)
                .NotEmpty().WithMessage("DriverId is required.");
          
        }
    }
}