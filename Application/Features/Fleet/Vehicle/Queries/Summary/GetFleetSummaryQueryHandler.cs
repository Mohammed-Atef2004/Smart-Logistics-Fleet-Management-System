using Application.Features.Fleet.Vehicle.DTOs;
using Domain.Fleet.Enums;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.Summary
{
    public class GetFleetSummaryHandler : IRequestHandler<GetFleetSummaryQuery, FleetSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFleetSummaryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FleetSummaryDto> Handle(GetFleetSummaryQuery request, CancellationToken ct)
        {
            var totalVehicles = await _unitOfWork.Vehicles.CountAsync();

            var vehiclesInMaintenance = await _unitOfWork.Vehicles
                .CountAsync(v => v.Status == VehicleStatus.InMaintenance);

            var vehiclesOnTrip = await _unitOfWork.Vehicles
                .CountAsync(v => v.Status == VehicleStatus.OnTrip);

            var totalDrivers = await _unitOfWork.Drivers.CountAsync();

            var availableDrivers = await _unitOfWork.Drivers
                .CountAsync(d => d.IsActive && d.CurrentVehicleId == null);

            return new FleetSummaryDto(
                TotalVehicles: totalVehicles,
                VehiclesInMaintenance: vehiclesInMaintenance,
                VehiclesOnTrip: vehiclesOnTrip,
                TotalDrivers: totalDrivers,
                AvailableDrivers: availableDrivers
            );
        }
    } 
}
