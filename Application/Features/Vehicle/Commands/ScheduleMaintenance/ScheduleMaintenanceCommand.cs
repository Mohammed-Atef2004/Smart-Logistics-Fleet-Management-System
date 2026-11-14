namespace Application.Features.Vehicle.Commands.ScheduleMaintenance
{
    public record ScheduleMaintenanceCommand(
    Guid VehicleId,
    DateTime ScheduledDate,
    string Description) : IRequest<Result>;
}