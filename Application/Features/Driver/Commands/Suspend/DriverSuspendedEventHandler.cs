using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.Suspend
{
    public class DriverSuspendedEventHandler : INotificationHandler<DriverSuspendedEvent>
    {
        private readonly ILogger<DriverSuspendedEventHandler> _logger;
        public DriverSuspendedEventHandler(ILogger<DriverSuspendedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverSuspendedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id} has been suspended.");
            return Task.CompletedTask;
        }
    }
}
