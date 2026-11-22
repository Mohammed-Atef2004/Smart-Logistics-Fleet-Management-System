using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.ClearShift;

public sealed class DriverShiftClearedEventHandler : INotificationHandler<DriverShiftClearedEvent>
{
    private readonly ILogger<DriverShiftClearedEventHandler> _logger;

    public DriverShiftClearedEventHandler(ILogger<DriverShiftClearedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverShiftClearedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Driver {DriverId} released shift {ShiftId}",
            notification.DriverId.Value, notification.ShiftId.Value);

        return Task.CompletedTask;
    }
}
