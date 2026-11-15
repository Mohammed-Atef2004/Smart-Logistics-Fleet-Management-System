using Application.Features.Vehicle.Commands.CompleteMaintenance;
using Domain.Vehicles.Events;

namespace Application.Features.Vehicle.Commands.CompleteMaintenance;

public sealed class MaintenancePerformedEventHandler : INotificationHandler<MaintenancePerformedEvent>
{
    private readonly ILogger<MaintenancePerformedEventHandler> _logger;

    public MaintenancePerformedEventHandler(ILogger<MaintenancePerformedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MaintenancePerformedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Maintenance performed on Vehicle {VehicleId} (schedule {ScheduleId}).",
            notification.VehicleId.Value,
            notification.ScheduleId);

        return Task.CompletedTask;
    }
}
