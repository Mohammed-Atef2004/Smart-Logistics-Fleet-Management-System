using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.Terminate;

public sealed class DriverTerminatedEventHandler : INotificationHandler<DriverTerminatedEvent>
{
    private readonly ILogger<DriverTerminatedEventHandler> _logger;

    public DriverTerminatedEventHandler(ILogger<DriverTerminatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverTerminatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Driver {DriverId} terminated", notification.DriverId.Value);
        return Task.CompletedTask;
    }
}
