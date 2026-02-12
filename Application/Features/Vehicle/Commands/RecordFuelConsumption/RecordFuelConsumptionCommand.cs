using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.RecordFuelConsumption
{
    public record RecordFuelConsumptionCommand(
    Guid VehicleId,
    decimal Liters,
    decimal OdometerReading) : IRequest<Result>;
}
