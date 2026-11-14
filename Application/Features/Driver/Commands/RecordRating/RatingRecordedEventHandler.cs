namespace Application.Features.Driver.Commands.RecordRating
{
    public sealed class RatingRecordedEventHandler: INotificationHandler<DriverPerformanceDroppedEvent>
    {
        private readonly ILogger<DriverPerformanceDroppedEvent> _logger;
        public RatingRecordedEventHandler(ILogger<DriverPerformanceDroppedEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverPerformanceDroppedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Rating Recorded: DriverId: {notification.Id}, Rating: {notification.Value}");
            return Task.CompletedTask;
        }
    }
}