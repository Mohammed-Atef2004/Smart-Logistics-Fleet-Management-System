using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.Reactivate
{
    public class DriverReactivatedEventHandler:INotificationHandler<DriverReactivatedEvent>
    {
        private readonly ILogger<DriverReactivatedEvent> _logger;
        public DriverReactivatedEventHandler(ILogger<DriverReactivatedEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverReactivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id} has been reactivated.");
            return Task.CompletedTask;
        }
    }
}
