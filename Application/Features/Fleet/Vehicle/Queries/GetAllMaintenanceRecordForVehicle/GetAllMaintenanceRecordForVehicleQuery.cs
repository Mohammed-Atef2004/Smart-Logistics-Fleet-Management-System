using Amazon.Runtime.Internal;
using Application.Features.Fleet.MaintenanceRecord.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.GetAllMaintenanceRecordForVehicle
{
    public record GetAllMaintenanceRecordForVehicleQuery(Guid Id):IRequest<List<MaintenanceRecordDto>>;

}
