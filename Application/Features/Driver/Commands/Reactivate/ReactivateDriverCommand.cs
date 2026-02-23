using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.Reactivate
{
    public record ReactivateDriverCommand(DriverId Id) : IRequest<Unit>;

}
