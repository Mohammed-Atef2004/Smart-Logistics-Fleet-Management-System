using Application.Features.Vehicle.Commands.UpdateVehicleStatus;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;
using MediatR;

namespace Application.Features.Vehicles.Commands.UpdateVehicleStatus;

public sealed class UpdateVehicleStatusHandler : IRequestHandler<UpdateVehicleStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVehicleStatusCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(new Error("Vehicle.NotFound", "Vehicle not found."));

        var result = vehicle.UpdateStatus(request.NewStatus);

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}