using Domain.Drivers;
using Domain.Drivers.ValueObjects;
using Domain.Shifts.Events;
using Domain.Shifts.ValueObjects;

namespace Application.Features.Shift.EventHandlers;

/// <summary>
/// Keeps the Driver aggregate in sync when a shift closes.
/// <para>
/// The old module never released the driver's <c>CurrentShiftId</c>: after a
/// shift was completed, cancelled, or aborted, the driver still looked busy
/// and could not be assigned new work. All three closing events share the
/// same reaction, so they are handled by one class.
/// </para>
/// <para>
/// Handlers run after the commit (see <c>DomainEventInterceptor</c>), so this
/// is a separate unit of work. A missing driver is logged, not thrown: the
/// shift is already persisted and must not be rolled back.
/// </para>
/// </summary>
public sealed class ShiftClosedEventHandler :
    INotificationHandler<ShiftCompletedEvent>,
    INotificationHandler<ShiftCancelledEvent>,
    INotificationHandler<ShiftAbortedEvent>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ShiftClosedEventHandler> _logger;

    public ShiftClosedEventHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork,
        ILogger<ShiftClosedEventHandler> logger)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public Task Handle(ShiftCompletedEvent notification, CancellationToken cancellationToken)
        => ReleaseDriverShiftAsync(notification.DriverId, notification.ShiftId, cancellationToken);

    public Task Handle(ShiftCancelledEvent notification, CancellationToken cancellationToken)
        => ReleaseDriverShiftAsync(notification.DriverId, notification.ShiftId, cancellationToken);

    public Task Handle(ShiftAbortedEvent notification, CancellationToken cancellationToken)
        => ReleaseDriverShiftAsync(notification.DriverId, notification.ShiftId, cancellationToken);

    private async Task ReleaseDriverShiftAsync(
        DriverId driverId,
        ShiftId shiftId,
        CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId, cancellationToken);
        if (driver is null)
        {
            _logger.LogWarning(
                "Shift {ShiftId} closed but driver {DriverId} was not found; nothing to release",
                shiftId.Value, driverId.Value);
            return;
        }

        // Nothing to do when the driver already moved on (or the event is
        // stale). Skipping the save avoids a pointless write on every event.
        if (driver.CurrentShiftId is null || driver.CurrentShiftId.Value != shiftId)
            return;

        var result = driver.ReleaseShift(shiftId);
        if (result.IsFailure)
        {
            _logger.LogWarning(
                "Could not release shift {ShiftId} from driver {DriverId}: {ErrorCode}",
                shiftId.Value, driverId.Value, result.Error.Code);
            return;
        }

        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
