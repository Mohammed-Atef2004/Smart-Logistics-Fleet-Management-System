using Domain.Drivers.Events;

namespace Application.Features.Driver.Commands.UpdateLicense;

public sealed class LicenseUpdatedEventHandler : INotificationHandler<DriverLicenseUpdatedEvent>
{
    private readonly ILogger<LicenseUpdatedEventHandler> _logger;

    public LicenseUpdatedEventHandler(ILogger<LicenseUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DriverLicenseUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Driver {DriverId} license updated: {LicenseNumber}, expires {ExpiryDate:yyyy-MM-dd}",
            notification.DriverId.Value, notification.LicenseNumber, notification.ExpiryDate);

        return Task.CompletedTask;
    }
}
