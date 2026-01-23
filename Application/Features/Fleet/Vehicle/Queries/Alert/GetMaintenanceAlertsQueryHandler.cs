using Application.Features.Fleet.Vehicle.DTOs;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.Alert
{
    public class GetMaintenanceAlertsQueryHandler: IRequestHandler<GetMaintenanceAlertsQuery, List<MaintenanceAlertDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetMaintenanceAlertsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<MaintenanceAlertDto>> Handle(GetMaintenanceAlertsQuery request, CancellationToken ct)
        {
            return await _unitOfWork.Vehicles.EntityQuery
                .Where(v => v.CurrentMileage - v.LastMaintenanceMileage >= 10000) 
                .Select(v => new MaintenanceAlertDto(
                    v.Id,
                    v.LicensePlate,
                    v.CurrentMileage,
                    v.LastMaintenanceMileage,
                    v.CurrentMileage - v.LastMaintenanceMileage - 10000
                ))
                .ToListAsync();
        }
    }
}
