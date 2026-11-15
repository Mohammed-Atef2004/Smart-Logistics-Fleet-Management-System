using Domain.SharedKernel;

namespace Application.Features.Vehicle.Commands.RescheduleMaintenance;

/// <summary>
/// Moves a not-yet-performed maintenance schedule to a new date.
/// Vehicle.RescheduleMaintenance was previously unreachable.
/// </summary>
public record RescheduleMaintenanceCommand(
    Guid VehicleId,
    Guid ScheduleId,
    DateTime NewDate) : IRequest<Result>;
