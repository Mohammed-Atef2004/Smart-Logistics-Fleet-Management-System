using Amazon.Runtime.Internal;
using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.Susbend
{
    public record SuspendDriverCommand(DriverId Id, DriverSuspensionReason Reason) : IRequest<Unit>;

}
