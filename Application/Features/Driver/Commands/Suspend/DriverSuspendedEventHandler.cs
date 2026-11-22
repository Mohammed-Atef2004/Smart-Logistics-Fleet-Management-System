using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.Suspend;

public sealed class DriverSuspendedEventHandler : INotificationHandler<DriverSuspendedEvent>
{
    private readonly ILogger<DriverSuspendedEventHandler> _logger;

    public DriverSuspendedEventHandler(ILogger<DriverSuspendedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverSuspendedEvent notification, CancellationToken cancellationToken)
    {
        // The reason used to live in a private field; handlers could only
        // log the event's own GUID. It is now part of the payload.
        _logger.LogInformation("Driver {DriverId} suspended. Reason: {Reason}",
            notification.DriverId.Value, notification.Reason);

        return Task.CompletedTask;
    }
}
