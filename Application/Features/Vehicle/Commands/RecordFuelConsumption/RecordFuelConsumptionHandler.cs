using Application.Features.Vehicle.Commands.RecordFuelConsumption;
using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;
using MediatR;

namespace Application.Features.Vehicles.Commands.RecordFuelConsumption;

public sealed class RecordFuelConsumptionCommandHandler : IRequestHandler<RecordFuelConsumptionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RecordFuelConsumptionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RecordFuelConsumptionCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

        if (vehicle is null)
            return Result.Failure(new Error("Vehicle.NotFound", "Vehicle not found."));

        var fuelResult = FuelConsumption.Create(request.Liters, request.OdometerReading);
        if (fuelResult.IsFailure)
            return Result.Failure(fuelResult.Error);

        
        var result = vehicle.RecordFuelConsumption(fuelResult.Value);

        if (result.IsFailure)
            return result;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}