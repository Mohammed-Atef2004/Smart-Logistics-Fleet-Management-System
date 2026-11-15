using Application.Features.Vehicle.Commands.RegisterNewVehicle;
using Domain.Vehicles.Events;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle;

public sealed class VehicleRegisteredEventHandler : INotificationHandler<VehicleRegisteredEvent>
{
    private readonly ILogger<VehicleRegisteredEventHandler> _logger;

    public VehicleRegisteredEventHandler(ILogger<VehicleRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(VehicleRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "VehicleRegisteredEvent handled for VehicleId {VehicleId}",
            notification.VehicleId.Value);

        return Task.CompletedTask;
    }
}
