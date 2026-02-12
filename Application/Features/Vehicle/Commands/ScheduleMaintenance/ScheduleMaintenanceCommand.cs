using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.ScheduleMaintenance
{
    public record ScheduleMaintenanceCommand(
    Guid VehicleId,
    DateTime ScheduledDate,
    string Description) : IRequest<Result>;
}
