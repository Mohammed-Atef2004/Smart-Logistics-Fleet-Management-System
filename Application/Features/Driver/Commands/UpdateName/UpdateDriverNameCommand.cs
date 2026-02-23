using Amazon.Runtime.Internal;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.Commands.UpdateName
{
    public record UpdateDriverNameCommand(DriverId Id, string FullName) : IRequest<Unit>;

}
