using Domain.SharedKernel;
using Domain.Vehicles.ValueObjects;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle;

/// <summary>
/// Receives PRIMITIVES, not domain value objects: VehicleSpecification has
/// a private constructor (Result-pattern factory), so it cannot be bound
/// from HTTP JSON — the old command shape made POST /api/vehicles fail
/// during model binding. The handler builds the value objects and maps
/// their failures to Result errors.
/// </summary>
public record RegisterNewVehicleCommand(
    string PlateNumber,
    string Model,
    int Year,
    string EngineType) : IRequest<Result<VehicleId>>;
