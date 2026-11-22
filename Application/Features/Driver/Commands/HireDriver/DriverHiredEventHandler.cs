using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.HireDriver;

public sealed class DriverHiredEventHandler : INotificationHandler<DriverHiredEvent>
{
    private readonly ILogger<DriverHiredEventHandler> _logger;

    public DriverHiredEventHandler(ILogger<DriverHiredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverHiredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Driver {DriverId} hired: {FullName}",
            notification.DriverId.Value, notification.FullName);

        return Task.CompletedTask;
    }
}
