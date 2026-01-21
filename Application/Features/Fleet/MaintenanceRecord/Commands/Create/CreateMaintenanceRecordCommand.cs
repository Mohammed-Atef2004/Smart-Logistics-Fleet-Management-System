using Amazon.Runtime.Internal;
using Application.Features.Fleet.MaintenanceRecord.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.Commands.Create
{
    public record CreateMaintenanceRecordCommand(MaintenanceRecordDto MaintenanceRecordDto):IRequest<Guid>;
    
}
