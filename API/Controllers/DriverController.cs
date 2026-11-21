using Application.Features.Driver.Commands.AssignShift;
using Application.Features.Driver.Commands.ClearShift;
using Application.Features.Driver.Commands.HireDriver;
using Application.Features.Driver.Commands.Reactivate;
using Application.Features.Driver.Commands.RecordRating;
using Application.Features.Driver.Commands.Suspend;
using Application.Features.Driver.Commands.Terminate;
using Application.Features.Driver.Commands.UpdateLicense;
using Application.Features.Driver.Commands.UpdateName;
using Application.Features.Driver.Queries.GetAll;
using Application.Features.Driver.Queries.GetById;
using Domain.Drivers.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Driver endpoints. The id always comes from the route and is injected
/// into the command — the old actions bound the whole command from the
/// body while ignoring the route id, so a caller could update a driver
/// other than the one in the URL.
/// </summary>
[ApiController]
[Route("api/drivers")]
public sealed class DriversController : ApiController
{
    public DriversController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllDriversQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{driverId:guid}")]
    public async Task<IActionResult> GetById(Guid driverId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetDriverByIdQuery(driverId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    public async Task<IActionResult> Hire([FromBody] HireDriverCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { driverId = result.Value }, result.Value);
    }

    [HttpPut("{driverId:guid}/name")]
    public async Task<IActionResult> UpdateName(
        Guid driverId, [FromBody] UpdateDriverNameRequest request, CancellationToken ct)
    {
        var command = new UpdateDriverNameCommand(driverId, request.FullName);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPut("{driverId:guid}/license")]
    public async Task<IActionResult> UpdateLicense(
        Guid driverId, [FromBody] UpdateDriverLicenseRequest request, CancellationToken ct)
    {
        var command = new UpdateDriverLicenseCommand(
            driverId, request.LicenseNumber, request.ExpiryDate, request.Category);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{driverId:guid}/suspend")]
    public async Task<IActionResult> Suspend(
        Guid driverId, [FromBody] SuspendDriverRequest request, CancellationToken ct)
    {
        var command = new SuspendDriverCommand(driverId, request.Reason);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{driverId:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid driverId, CancellationToken ct)
    {
        var result = await _sender.Send(new ReactivateDriverCommand(driverId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{driverId:guid}/terminate")]
    public async Task<IActionResult> Terminate(Guid driverId, CancellationToken ct)
    {
        var result = await _sender.Send(new TerminateDriverCommand(driverId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{driverId:guid}/ratings")]
    public async Task<IActionResult> RecordRating(
        Guid driverId, [FromBody] RecordDriverRatingRequest request, CancellationToken ct)
    {
        var command = new RecordDriverRatingCommand(driverId, request.Rating);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{driverId:guid}/shifts")]
    public async Task<IActionResult> AssignShift(
        Guid driverId, [FromBody] AssignShiftRequest request, CancellationToken ct)
    {
        var command = new AssignDriverToShiftCommand(driverId, request.Start, request.End);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpDelete("{driverId:guid}/shift")]
    public async Task<IActionResult> ClearShift(Guid driverId, CancellationToken ct)
    {
        var result = await _sender.Send(new ClearDriverShiftCommand(driverId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }
}

#region Request DTOs

public sealed record UpdateDriverNameRequest(string FullName);

public sealed record UpdateDriverLicenseRequest(string LicenseNumber, DateTime ExpiryDate, string Category);

public sealed record SuspendDriverRequest(DriverSuspensionReason Reason);

public sealed record RecordDriverRatingRequest(double Rating);

public sealed record AssignShiftRequest(DateTime Start, DateTime End);

#endregion
