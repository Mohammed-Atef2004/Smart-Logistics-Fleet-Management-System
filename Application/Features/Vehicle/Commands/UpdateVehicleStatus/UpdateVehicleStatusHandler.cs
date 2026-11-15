using Domain.Vehicles;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Commands.UpdateVehicleStatus;

public sealed class UpdateVehicleStatusHandler : IRequestHandler<UpdateVehicleStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;

    public UpdateVehicleStatusHandler(IUnitOfWork unitOfWork, IVehicleRepository vehicleRepository)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result> Handle(UpdateVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(VehicleErrors.NotFound(request.VehicleId));

        var result = vehicle.UpdateStatus(request.NewStatus);

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
