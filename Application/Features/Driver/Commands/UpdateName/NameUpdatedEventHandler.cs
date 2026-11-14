using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.UpdateName
{
    public sealed class NameUpdatedEventHandler: INotificationHandler<DriverNameUpdatedEvent>
    {
        private readonly ILogger<DriverNameUpdatedEvent> _logger;
        public NameUpdatedEventHandler(ILogger<DriverNameUpdatedEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverNameUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id}");
            return Task.CompletedTask;
        }
    }
}