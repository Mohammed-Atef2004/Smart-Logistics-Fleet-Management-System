using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.UpdateLicence
{
    public class LicenceUpdatedEventHandler : INotificationHandler<DriverLicenseUpdatedEvent>
    {
        private readonly ILogger<DriverLicenseUpdatedEvent> _logger;
        public LicenceUpdatedEventHandler(ILogger<DriverLicenseUpdatedEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverLicenseUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id} has updated their license.");
            return Task.CompletedTask;
        }
    }
}
