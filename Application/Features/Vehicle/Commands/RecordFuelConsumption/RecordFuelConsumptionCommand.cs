namespace Application.Features.Vehicle.Commands.RecordFuelConsumption
{
    public record RecordFuelConsumptionCommand(
    Guid VehicleId,
    decimal Liters,
    decimal OdometerReading) : IRequest<Result>;
}