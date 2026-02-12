using Application.Features.Vehicle.Commands.RetireVehicle;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;
using MediatR;

namespace Application.Features.Vehicles.Commands.RetireVehicle;

public sealed class RetireVehicleCommandHandler : IRequestHandler<RetireVehicleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RetireVehicleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RetireVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(new Error("Vehicle.NotFound", "Vehicle not found."));

        
        var result = vehicle.Retire();

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}