using Domain.Vehicles;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle;

public sealed class RegisterNewVehicleCommandHandler
    : IRequestHandler<RegisterNewVehicleCommand, Result<VehicleId>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleUniquenessChecker _vehicleUniquenessChecker;
    private readonly ILogger<RegisterNewVehicleCommandHandler> _logger;

    public RegisterNewVehicleCommandHandler(
        IUnitOfWork unitOfWork,
        IVehicleRepository vehicleRepository,
        IVehicleUniquenessChecker vehicleUniquenessChecker,
        ILogger<RegisterNewVehicleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _vehicleRepository = vehicleRepository;
        _vehicleUniquenessChecker = vehicleUniquenessChecker;
        _logger = logger;
    }

    public async Task<Result<VehicleId>> Handle(RegisterNewVehicleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting vehicle registration for {Plate}", request.PlateNumber);

        // Every value-object construction result must be checked before
        // .Value — the old code fed an invalid plate straight into the
        // aggregate (a hidden invalid-state bug).
        var plateResult = VehiclePlateNumber.Create(request.PlateNumber);
        if (plateResult.IsFailure)
        {
            _logger.LogWarning("Vehicle registration failed: {Error}", plateResult.Error);
            return Result<VehicleId>.Failure(plateResult.Error);
        }

        var specResult = VehicleSpecification.Create(request.Model, request.Year, request.EngineType);
        if (specResult.IsFailure)
        {
            _logger.LogWarning("Vehicle registration failed: {Error}", specResult.Error);
            return Result<VehicleId>.Failure(specResult.Error);
        }

        var vehicleResult = Domain.Vehicles.Vehicle.Register(plateResult.Value, specResult.Value, _vehicleUniquenessChecker);
        if (vehicleResult.IsFailure)
        {
            _logger.LogWarning("Vehicle registration failed: {Error}", vehicleResult.Error);
            return Result<VehicleId>.Failure(vehicleResult.Error);
        }

        await _vehicleRepository.AddAsync(vehicleResult.Value);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Vehicle registered with Id {VehicleId}", vehicleResult.Value.Id.Value);
        return Result<VehicleId>.Success(vehicleResult.Value.Id);
    }
}
