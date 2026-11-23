using Application.Features.Shift.Commands.AbortShift;
using Application.Features.Shift.Commands.CancelShift;
using Application.Features.Shift.Commands.CompleteShift;
using Application.Features.Shift.Commands.Create;
using Application.Features.Shift.Commands.RescheduleShift;
using Application.Features.Shift.Commands.StartShift;
using Application.Features.Shift.Queries.GetAll;
using Application.Features.Shift.Queries.GetById;
using Domain.Shifts.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Shift endpoints. The id always comes from the route and is injected into
/// the command - the old actions bound domain value objects straight from
/// the route, which the model binder cannot construct.
/// </summary>
[ApiController]
[Route("api/shifts")]
public sealed class ShiftsController : ApiController
{
    public ShiftsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? driverId,
        [FromQuery] ShiftStatus? status,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllShiftsQuery(driverId, status), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{shiftId:guid}")]
    public async Task<IActionResult> GetById(Guid shiftId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetShiftByIdQuery(shiftId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShiftRequest request, CancellationToken ct)
    {
        var command = new CreateShiftCommand(request.DriverId, request.Start, request.End);
        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { shiftId = result.Value }, result.Value);
    }

    [HttpPost("{shiftId:guid}/start")]
    public async Task<IActionResult> Start(Guid shiftId, CancellationToken ct)
    {
        var result = await _sender.Send(new StartShiftCommand(shiftId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shiftId:guid}/complete")]
    public async Task<IActionResult> Complete(Guid shiftId, CancellationToken ct)
    {
        var result = await _sender.Send(new CompleteShiftCommand(shiftId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shiftId:guid}/abort")]
    public async Task<IActionResult> Abort(Guid shiftId, CancellationToken ct)
    {
        var result = await _sender.Send(new AbortShiftCommand(shiftId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shiftId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid shiftId, CancellationToken ct)
    {
        var result = await _sender.Send(new CancelShiftCommand(shiftId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPut("{shiftId:guid}")]
    public async Task<IActionResult> Reschedule(
        Guid shiftId, [FromBody] RescheduleShiftRequest request, CancellationToken ct)
    {
        var command = new RescheduleShiftCommand(shiftId, request.Start, request.End);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }
}

#region Request DTOs

public sealed record CreateShiftRequest(Guid DriverId, DateTime Start, DateTime End);

public sealed record RescheduleShiftRequest(DateTime Start, DateTime End);

#endregion
