using Domain.Vehicles;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Commands.RetireVehicle;

public sealed class RetireVehicleCommandHandler : IRequestHandler<RetireVehicleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;

    public RetireVehicleCommandHandler(IUnitOfWork unitOfWork, IVehicleRepository vehicleRepository)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result> Handle(RetireVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(VehicleErrors.NotFound(request.VehicleId));

        var result = vehicle.Retire();

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
