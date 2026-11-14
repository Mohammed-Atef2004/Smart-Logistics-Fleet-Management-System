using Domain.Vehicles;
using Application.Features.Vehicle.Commands.RecordFuelConsumption;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicles.Commands.RecordFuelConsumption;

public sealed class RecordFuelConsumptionCommandHandler : IRequestHandler<RecordFuelConsumptionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;

    public RecordFuelConsumptionCommandHandler(IUnitOfWork unitOfWork, IVehicleRepository vehicleRepository)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result> Handle(RecordFuelConsumptionCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(new VehicleId(request.VehicleId), cancellationToken);

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