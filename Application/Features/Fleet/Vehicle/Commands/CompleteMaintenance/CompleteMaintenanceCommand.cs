using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.CompleteMaintenance
{
    public record CompleteMaintenanceCommand(Guid VehicleId) : IRequest<Unit>;
}
