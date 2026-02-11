using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle
{
    public class VehicleRegisteredEventHandler : INotificationHandler<VehicleRegisteredEvent>
    {
        private readonly ILogger<VehicleRegisteredEventHandler> _logger;

        public VehicleRegisteredEventHandler(ILogger<VehicleRegisteredEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(VehicleRegisteredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "VehicleRegisteredEvent handled for VehicleId {VehicleId}",
                notification.Id
            );

            // you can add any action here 

            return Task.CompletedTask;
        }
    }
}
