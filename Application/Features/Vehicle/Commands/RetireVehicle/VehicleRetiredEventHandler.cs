using Domain.Vehicles.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Vehicles.Events;

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
            notification.id);
        // Here I would add logic to archive related records, notify other systems, etc.
        return Task.CompletedTask;
    }
}