using Domain.SharedKernel;

namespace Application.Features.Vehicle.Commands.CompleteMaintenance;

/// <summary>
/// Closes the maintenance loop: marks a schedule as performed and puts
/// the vehicle back in service. This use case did not exist before —
/// Vehicle.CompleteMaintenance was unreachable dead behavior.
/// </summary>
public record CompleteMaintenanceCommand(
    Guid VehicleId,
    Guid ScheduleId,
    string? Remarks) : IRequest<Result>;
