using Application.Features.Vehicle.DTOs;
using Domain.Vehicles;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Queries.GetById;

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
            return Result<VehicleDetailsDto>.Failure(VehicleErrors.NotFound(request.VehicleId));

        // Last maintenance = the most recent COMPLETED schedule. The old
        // handler loaded the schedules and then always returned null.
        DateTime? lastMaintenanceDate = vehicle.MaintenanceSchedules
            .Where(s => s.IsCompleted)
            .Select(s => s.CompletedAt)
            .DefaultIfEmpty(null)
            .Max();

        var dto = new VehicleDetailsDto(
            Id: vehicle.Id.Value,
            PlateNumber: vehicle.PlateNumber.Value,
            Status: vehicle.Status.ToString(),
            Model: vehicle.Specification.Model,
            Year: vehicle.Specification.Year,
            LastMaintenanceDate: lastMaintenanceDate
        );

        return Result<VehicleDetailsDto>.Success(dto);
    }
}
