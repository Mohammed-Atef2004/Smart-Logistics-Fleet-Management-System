namespace Application.Features.Driver.Commands.UpdateLicence
{
    public sealed class LicenceUpdatedEventHandler : INotificationHandler<DriverLicenseUpdatedEvent>
    {
        private readonly ILogger<DriverLicenseUpdatedEvent> _logger;
        public LicenceUpdatedEventHandler(ILogger<DriverLicenseUpdatedEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverLicenseUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id} has updated their license.");
            return Task.CompletedTask;
        }
    }
}