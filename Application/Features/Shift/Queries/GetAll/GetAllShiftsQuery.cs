using Application.Features.Shift.DTOs;
using Domain.Shifts.Enums;

namespace Application.Features.Shift.Queries.GetAll;

/// <summary>
/// Lists shifts, optionally filtered by driver and status. Both filters are
/// nullable so the endpoint can serve "all shifts", "one driver's shifts",
/// and "all active shifts" without extra endpoints.
/// </summary>
public record GetAllShiftsQuery(Guid? DriverId = null, ShiftStatus? Status = null)
    : IRequest<Result<List<ShiftDto>>>;
