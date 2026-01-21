using Application.Features.Fleet.MaintenanceRecord.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Update
{
    public record UpdateMaintenanceRecordCommand(Guid Id,UpdateMaintenanceRecordDto UpdateMaintenanceRecordDto):IRequest<Unit>;
    
}
