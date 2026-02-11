using Domain.Vehicles.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Vehicles.Events;

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
         notification.Id.Value, 
         notification.NewStatus);

        return Task.CompletedTask;
    }
}