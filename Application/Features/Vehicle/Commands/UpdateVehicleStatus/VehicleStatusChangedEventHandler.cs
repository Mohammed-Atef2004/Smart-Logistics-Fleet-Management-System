using Application.Features.Vehicle.Commands.UpdateVehicleStatus;
using Domain.Vehicles.Events;

namespace Application.Features.Vehicle.Commands.UpdateVehicleStatus;

public sealed class VehicleStatusChangedEventHandler : INotificationHandler<VehicleStatusChangedEvent>
{
    private readonly ILogger<VehicleStatusChangedEventHandler> _logger;

    public VehicleStatusChangedEventHandler(ILogger<VehicleStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(VehicleStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Vehicle {VehicleId} status changed to {Status}",
            notification.VehicleId.Value,
            notification.NewStatus);

        return Task.CompletedTask;
    }
}
