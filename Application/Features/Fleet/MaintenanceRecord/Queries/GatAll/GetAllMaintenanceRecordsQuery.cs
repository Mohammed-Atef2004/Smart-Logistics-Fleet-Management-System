using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Domain.Vehicles.Enums;
using MediatR;

namespace Application.Features.Fleet.MaintenanceRecord.Queries.GetAll;

public record GetAllMaintenanceRecordsQuery(
    Guid? VehicleId = null,         
    MaintenanceType? Type = null,      
    DateTime? Date = null         
            
) : IRequest<List<MaintenanceRecordDto>>;