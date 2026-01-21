using Amazon.Runtime.Internal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Delete
{
    public record DeleteMaintenanceRecordCommand(Guid Id):IRequest<Unit>;
    

}
