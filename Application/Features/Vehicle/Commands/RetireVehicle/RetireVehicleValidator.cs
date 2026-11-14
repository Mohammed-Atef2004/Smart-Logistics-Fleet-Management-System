namespace Application.Features.Vehicle.Commands.RetireVehicle
{

    public class RetireVehicleValidator : AbstractValidator<RetireVehicleCommand>
    {
        public RetireVehicleValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
        }
    }
}