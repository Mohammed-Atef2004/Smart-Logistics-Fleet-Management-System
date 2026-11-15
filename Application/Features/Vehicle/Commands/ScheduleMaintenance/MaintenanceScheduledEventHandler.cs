using Application.Features.Vehicle.Commands.ScheduleMaintenance;
using Domain.Vehicles.Events;

namespace Application.Features.Vehicle.Commands.ScheduleMaintenance;

public sealed class MaintenanceScheduledEventHandler : INotificationHandler<MaintenanceScheduledEvent>
{
    private readonly ILogger<MaintenanceScheduledEventHandler> _logger;

    public MaintenanceScheduledEventHandler(ILogger<MaintenanceScheduledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MaintenanceScheduledEvent notification, CancellationToken cancellationToken)
    {
        // The old handler logged notification.OccurredOnUtc (the moment the
        // event was raised) instead of the actual maintenance date.
        _logger.LogInformation("Vehicle {VehicleId} has been scheduled for maintenance on {ScheduledDate} (schedule {ScheduleId}).",
            notification.VehicleId.Value, notification.ScheduledDate, notification.ScheduleId);

        return Task.CompletedTask;
    }
}
