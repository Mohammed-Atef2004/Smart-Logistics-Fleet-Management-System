using Domain.Vehicles;
using Application.Features.Vehicle.DTOs;

namespace Application.Features.Vehicle.Queries.GetById
{
    public sealed class GetVehicleDetailsHandler
     : IRequestHandler<GetVehicleDetailsQuery, Result<VehicleDetailsDto>>
    {
        private readonly IVehicleRepository _vehicleRepository;

        public GetVehicleDetailsHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<Result<VehicleDetailsDto>> Handle(
     GetVehicleDetailsQuery request,
     CancellationToken cancellationToken)
        {
            var vehicleId = new VehicleId(request.VehicleId);

            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);

            if (vehicle is null)
            {
                return Result<VehicleDetailsDto>.Failure(
                    new Error("Vehicle.NotFound", $"Vehicle with ID {request.VehicleId} was not found."));
            }

            var dto = new VehicleDetailsDto(
                Id: vehicle.Id.Value,
                PlateNumber: vehicle.PlateNumber.Value,
                Status: vehicle.Status.ToString(),
                Model: vehicle.Specification.Model,
                Year: vehicle.Specification.Year,
                LastMaintenanceDate: null
            );

            return Result<VehicleDetailsDto>.Success(dto);
        }
    }
}