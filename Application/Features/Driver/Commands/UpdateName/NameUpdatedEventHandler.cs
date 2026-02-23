using Microsoft.Extensions.Logging;
using Domain.Drivers.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.UpdateName
{
    public class NameUpdatedEventHandler: INotificationHandler<DriverNameUpdatedEvent>
    {
        private readonly ILogger<DriverNameUpdatedEvent> _logger;
        public NameUpdatedEventHandler(ILogger<DriverNameUpdatedEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverNameUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver with Id {notification.Id}");
            return Task.CompletedTask;
        }
    }
}
