using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.AssignShift;

public sealed class DriverShiftAssignedEventHandler : INotificationHandler<DriverShiftAssignedEvent>
{
    private readonly ILogger<DriverShiftAssignedEventHandler> _logger;

    public DriverShiftAssignedEventHandler(ILogger<DriverShiftAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverShiftAssignedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Driver {DriverId} assigned to shift {ShiftId}",
            notification.DriverId.Value, notification.ShiftId.Value);

        return Task.CompletedTask;
    }
}
