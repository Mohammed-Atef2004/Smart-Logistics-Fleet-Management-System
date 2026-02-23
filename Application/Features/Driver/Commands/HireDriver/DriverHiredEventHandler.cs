using Microsoft.Extensions.Logging;
using Domain.Drivers.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.HireDriver
{
    public class DriverHiredEventHandler: INotificationHandler<DriverHiredEvent>
    {
        private ILogger<DriverHiredEvent> _logger;
        public DriverHiredEventHandler(ILogger<DriverHiredEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(DriverHiredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Driver Hired: {notification.Name}");
            return Task.CompletedTask;
        }
    }
}
