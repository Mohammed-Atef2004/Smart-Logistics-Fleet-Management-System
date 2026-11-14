namespace Application.Features.Vehicle.Queries.GetById
{
    public class GetVehicleDetailsValidator : AbstractValidator<GetVehicleDetailsQuery>
    {
        public GetVehicleDetailsValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty().WithMessage("Vehicle ID is required.");
        }
    }
}