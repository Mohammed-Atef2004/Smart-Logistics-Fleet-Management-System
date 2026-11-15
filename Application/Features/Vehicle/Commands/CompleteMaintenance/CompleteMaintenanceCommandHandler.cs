using Domain.Vehicles;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Commands.CompleteMaintenance;

public sealed class CompleteMaintenanceCommandHandler : IRequestHandler<CompleteMaintenanceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;

    public CompleteMaintenanceCommandHandler(IUnitOfWork unitOfWork, IVehicleRepository vehicleRepository)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result> Handle(CompleteMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(VehicleErrors.NotFound(request.VehicleId));

        MaintenanceRemarks? remarks = null;
        if (request.Remarks is not null)
        {
            var remarksResult = MaintenanceRemarks.Create(request.Remarks);
            if (remarksResult.IsFailure)
                return Result.Failure(remarksResult.Error);

            remarks = remarksResult.Value;
        }

        var result = vehicle.CompleteMaintenance(request.ScheduleId, remarks);

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
