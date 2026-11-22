using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.RecordRating;

public sealed class RatingRecordedEventHandler : INotificationHandler<DriverPerformanceDroppedEvent>
{
    private readonly ILogger<RatingRecordedEventHandler> _logger;

    public RatingRecordedEventHandler(ILogger<RatingRecordedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverPerformanceDroppedEvent notification, CancellationToken cancellationToken)
    {
        // Previously logged notification.Id (the event GUID) and read a
        // lowercase field; both are now real payload properties.
        _logger.LogWarning(
            "Driver {DriverId} performance dropped to {Rating} after {RatingCount} ratings",
            notification.DriverId.Value, notification.Rating, notification.RatingCount);

        return Task.CompletedTask;
    }
}
