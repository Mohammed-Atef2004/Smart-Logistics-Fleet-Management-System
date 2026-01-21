using Application.Features.Fleet.Vehicle.DTOs;
using Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.GetById
{
    public class GetVehicleDetailsHandler : IRequestHandler<GetVehicleDetailsQuery, VehicleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVehicleDetailsHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<VehicleDto> Handle(GetVehicleDetailsQuery request, CancellationToken cancellationToken)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.Id);

            return new VehicleDto
            {
                LicensePlate = vehicle.LicensePlate,
                Model = vehicle.Model,
                CurrentMileage = vehicle.CurrentMileage,
                IsMaintenanceDue = vehicle.CurrentMileage - vehicle.LastMaintenanceMileage >= 10000
            };
        }
    }
}
