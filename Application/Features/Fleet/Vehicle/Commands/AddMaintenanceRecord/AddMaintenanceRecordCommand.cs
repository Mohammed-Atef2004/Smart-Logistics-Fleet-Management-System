using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Application.Features.Fleet.Vehicle.DTOs;
using Domain.Fleet.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.AddMaintenanceRecord
{
    public record AddMaintenanceRecordCommand(MaintenanceRecordDto maintenanceRecordDto) : IRequest<Unit>;
}
