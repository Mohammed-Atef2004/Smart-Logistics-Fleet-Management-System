using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.HireDriver
{
    public sealed class DriverHiredEventHandler: INotificationHandler<DriverHiredEvent>
    {
        private ILogger<DriverHiredEvent> _logger;
        public DriverHiredEventHandler(ILogger<DriverHiredEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverHiredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver Hired: {notification.Name}");
            return Task.CompletedTask;
        }
    }
}