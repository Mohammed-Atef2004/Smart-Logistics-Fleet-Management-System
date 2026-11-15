using Domain.Vehicles;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Commands.ScheduleMaintenance;

public sealed class ScheduleMaintenanceCommandHandler : IRequestHandler<ScheduleMaintenanceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;

    public ScheduleMaintenanceCommandHandler(IUnitOfWork unitOfWork, IVehicleRepository vehicleRepository)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result> Handle(ScheduleMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(VehicleErrors.NotFound(request.VehicleId));

        var descriptionResult = MaintenanceDescription.Create(request.Description);
        if (descriptionResult.IsFailure)
            return Result.Failure(descriptionResult.Error);

        var result = vehicle.ScheduleMaintenance(request.ScheduledDate, descriptionResult.Value);

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
