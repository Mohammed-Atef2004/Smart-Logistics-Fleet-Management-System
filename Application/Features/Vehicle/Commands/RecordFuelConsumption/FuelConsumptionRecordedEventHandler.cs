using Domain.Vehicles.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Vehicles.Events;

public sealed class FuelConsumptionRecordedEventHandler : INotificationHandler<FuelConsumptionRecordedEvent>
{
    private readonly ILogger<FuelConsumptionRecordedEventHandler> _logger;

    public FuelConsumptionRecordedEventHandler(ILogger<FuelConsumptionRecordedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(FuelConsumptionRecordedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fuel consumption recorded for Vehicle {VehicleId}: {Liters}L at {Odometer} km.",
            notification.Id, notification.liters, notification.odometerReading);

        return Task.CompletedTask;
    }
}