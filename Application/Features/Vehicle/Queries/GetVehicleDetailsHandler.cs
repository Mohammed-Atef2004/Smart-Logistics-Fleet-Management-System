using Application.Features.Vehicles.DTOs;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Queries
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
                Id = vehicle.Id,
                LicensePlate = vehicle.LicensePlate,
                Model = vehicle.Model,
                Status = vehicle.Status.ToString(),
                CurrentMileage = vehicle.CurrentMileage,
                IsMaintenanceDue = (vehicle.CurrentMileage - vehicle.LastMaintenanceMileage >= 10000)
            };
        }
    }
}
