namespace Application.Features.Driver.Commands.Suspend
{
    public sealed class DriverSuspendedEventHandler : INotificationHandler<DriverSuspendedEvent>
    {
        private readonly ILogger<DriverSuspendedEventHandler> _logger;
        public DriverSuspendedEventHandler(ILogger<DriverSuspendedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverSuspendedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id} has been suspended.");
            return Task.CompletedTask;
        }
    }
}