using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.Deactivate
{
    public record DeactivateDriverCommand(Guid Id):IRequest<Unit>;

}
