using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.Reactivate;

public sealed class DriverReactivatedEventHandler : INotificationHandler<DriverReactivatedEvent>
{
    private readonly ILogger<DriverReactivatedEventHandler> _logger;

    public DriverReactivatedEventHandler(ILogger<DriverReactivatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverReactivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Driver {DriverId} reactivated", notification.DriverId.Value);
        return Task.CompletedTask;
    }
}
