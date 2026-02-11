using Domain.Vehicles.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Vehicles.Events;

public sealed class MaintenanceScheduledEventHandler : INotificationHandler<MaintenanceScheduledEvent>
{
    private readonly ILogger<MaintenanceScheduledEventHandler> _logger;

    public MaintenanceScheduledEventHandler(ILogger<MaintenanceScheduledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MaintenanceScheduledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Vehicle {VehicleId} has been scheduled for maintenance on {Date}",
            notification.Id, notification.OccurredOnUtc);

        return Task.CompletedTask;
    }
}