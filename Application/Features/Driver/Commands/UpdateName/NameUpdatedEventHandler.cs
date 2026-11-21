using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.UpdateName;

public sealed class NameUpdatedEventHandler : INotificationHandler<DriverNameUpdatedEvent>
{
    private readonly ILogger<NameUpdatedEventHandler> _logger;

    public NameUpdatedEventHandler(ILogger<NameUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverNameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        // The event existed with a handler but was never published; the
        // handler logged only the id. It now carries both names.
        _logger.LogInformation("Driver {DriverId} renamed from {OldName} to {NewName}",
            notification.DriverId.Value, notification.OldName, notification.NewName);

        return Task.CompletedTask;
    }
}
