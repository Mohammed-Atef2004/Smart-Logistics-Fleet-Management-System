namespace Application.Features.Vehicle.Commands.RecordFuelConsumption
{
  

    public class RecordFuelConsumptionValidator : AbstractValidator<RecordFuelConsumptionCommand>
    {
        public RecordFuelConsumptionValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
            RuleFor(x => x.Liters).GreaterThan(0);
            RuleFor(x => x.OdometerReading).GreaterThan(0);
        }
    }
}