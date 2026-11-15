using Application.Features.Vehicle.Commands.RecordFuelConsumption;
using Domain.Vehicles.Events;

namespace Application.Features.Vehicle.Commands.RecordFuelConsumption;

public sealed class FuelConsumptionRecordedEventHandler : INotificationHandler<FuelConsumptionRecordedEvent>
{
    private readonly ILogger<FuelConsumptionRecordedEventHandler> _logger;

    public FuelConsumptionRecordedEventHandler(ILogger<FuelConsumptionRecordedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(FuelConsumptionRecordedEvent notification, CancellationToken cancellationToken)
    {
        // Reads the event's own payload now; the old lowercase properties
        // (liters / odometerReading) logged null/wrong values and
        // notification.Id logged the base event GUID, not the vehicle id.
        _logger.LogInformation("Fuel consumption recorded for Vehicle {VehicleId}: {Liters}L at {Odometer} km.",
            notification.VehicleId.Value, notification.Liters, notification.OdometerReading);

        return Task.CompletedTask;
    }
}
