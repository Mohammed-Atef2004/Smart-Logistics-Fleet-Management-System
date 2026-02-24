using Application.Features.Driver.Commands.HireDriver;
using Application.Features.Driver.Commands.Reactivate;
using Application.Features.Driver.Commands.RecordRating;
using Application.Features.Driver.Commands.Susbend;
using Application.Features.Driver.Commands.UpdateLicence;
using Application.Features.Driver.Commands.UpdateName;
using Application.Features.Driver.Queries.GetById;
using Domain.Drivers.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriverController : ApiController
{
    private readonly IMediator _mediator;

    public DriverController(IMediator mediator) : base(mediator) { }


    // ===============================
    // Hire Driver (Command directly)
    // ===============================
    [HttpPost("hire")]
    public async Task<IActionResult> Hire([FromBody] HireDriverCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }

    // ===============================
    // Update Driver Name
    // ===============================
    [HttpPut("{driverId}/name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateDriverNameCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok();
    }

    // ===============================
    // Update Driver License
    // ===============================
    [HttpPut("{driverId}/license")]
    public async Task<IActionResult> UpdateLicense([FromBody] UpdateDriverLicenseCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok();
    }

    // ===============================
    // Suspend Driver
    // ===============================
    [HttpPost("{driverId}/suspend")]
    public async Task<IActionResult> Suspend([FromBody] SuspendDriverCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok();
    }

    // ===============================
    // Reactivate Driver
    // ===============================
    [HttpPost("{driverId}/reactivate")]
    public async Task<IActionResult> Reactivate([FromBody] ReactivateDriverCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok();
    }

    // ===============================
    // Assign Shift
    // ===============================
    //[HttpPost("assign-shift")]
    //public async Task<IActionResult> AssignShift([FromBody] AssignShiftToDriverCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    if (result.IsFailure) return BadRequest(result.Error);
    //    return Ok();
    //}

    // ===============================
    // Clear Shift
    // ===============================
    //[HttpPost("clear-shift")]
    //public async Task<IActionResult> ClearShift([FromBody] ClearDriverShiftCommand cmd)
    //{
    //    var result = await _mediator.Send(cmd);
    //    if (result.IsFailure) return BadRequest(result.Error);
    //    return Ok();
    //}

    // ===============================
    // Record Trip Rating
    // ===============================
    [HttpPost("record-rating")]
    public async Task<IActionResult> RecordRating([FromBody] RecordDriverRatingCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok();
    }

    // ===============================
    // Get Driver By Id (Query)
    // ===============================
    [HttpGet("{driverId}")]
    public async Task<IActionResult> GetDriver(DriverId driverId)
    {
        var query = new GetDriverByIdQuery(driverId);
        var dto = await _mediator.Send(query);

        if (dto == null) return NotFound();
        return Ok(dto);
    }
}