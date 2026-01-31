using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.RemovePackage
{
    public record RemovePackageCommand(Guid guid, Guid packageId) : IRequest<Unit>;
  
}
