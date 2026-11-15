using Application.Features.Vehicle.Commands.RetireVehicle;
using Domain.Vehicles.Events;

namespace Application.Features.Vehicle.Commands.RetireVehicle;

public sealed class VehicleRetiredEventHandler : INotificationHandler<VehicleRetiredEvent>
{
    private readonly ILogger<VehicleRetiredEventHandler> _logger;

    public VehicleRetiredEventHandler(ILogger<VehicleRetiredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(VehicleRetiredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Vehicle {VehicleId} has been retired. Archiving related records...",
            notification.VehicleId.Value);
        // Here I would add logic to archive related records, notify other systems, etc.
        return Task.CompletedTask;
    }
}
