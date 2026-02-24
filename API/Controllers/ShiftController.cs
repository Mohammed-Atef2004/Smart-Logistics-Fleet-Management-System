using Application.Features.Shift.Commands.Create;
using Application.Features.Shift.Commands.StartShift;
using Application.Features.Shift.Queries.GetById;
using Application.Features.Shift.Commands.CancelShift;
using Application.Features.Shift.Commands.CompleteShift;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Domain.Shifts.ValueObjects;
using Application.Features.Shift.Queries.GetAll;

namespace WebApi.Controllers;

public class ShiftsController : ApiController
{
    public ShiftsController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetShift(ShiftId id, CancellationToken ct)
    {
        var query = new GetShiftByIdQuery(id);
        var result = await _sender.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllShifts(CancellationToken ct)
    {
        var query = new GetAllShiftsQuery();
        var result = await _sender.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShiftCommand request, CancellationToken ct)
    {
        var command = new CreateShiftCommand(request.driverId, request.Start, request.End);
        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetShift), new { id = result.Value }, result.Value);
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(ShiftId id, CancellationToken ct)
    {
        var command = new StartShiftCommand(id);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(ShiftId id, CancellationToken ct)
    {
        var command = new CompleteShiftCommand(id);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(ShiftId id, CancellationToken ct)
    {
        var command = new CancelShiftCommand(id);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }
}
