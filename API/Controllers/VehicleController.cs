using Application.Features.Vehicle.Commands.RecordFuelConsumption;
using Application.Features.Vehicle.Commands.RegisterNewVehicle;
using Application.Features.Vehicle.Commands.RetireVehicle;
using Application.Features.Vehicle.Commands.ScheduleMaintenance;
using Application.Features.Vehicle.Commands.UpdateVehicleStatus;
using Application.Features.Vehicle.Queries.GetById;
using Application.Features.Vehicles.Commands.RecordFuelConsumption;
using Application.Features.Vehicles.Commands.RetireVehicle;
using Application.Features.Vehicles.Commands.ScheduleMaintenance;
using Application.Features.Vehicles.Commands.UpdateVehicleStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class VehiclesController : ApiController
{
    public VehiclesController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVehicleDetails(Guid id, CancellationToken ct)
    {
        var query = new GetVehicleDetailsQuery(id);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterNewVehicleCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/maintenance")]
    public async Task<IActionResult> ScheduleMaintenance(Guid id, [FromBody] ScheduleMaintenanceRequest request, CancellationToken ct)
    {
        var command = new ScheduleMaintenanceCommand(id, request.ScheduledDate, request.Description);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/fuel")]
    public async Task<IActionResult> RecordFuel(Guid id, [FromBody] RecordFuelConsumptionRequest request, CancellationToken ct)
    {
        var command = new RecordFuelConsumptionCommand(id, request.Liters, request.OdometerReading);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
    {
        var command = new UpdateVehicleStatusCommand(id, request.NewStatus);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/retire")]
    public async Task<IActionResult> Retire(Guid id, CancellationToken ct)
    {
        var command = new RetireVehicleCommand(id);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess ? Ok() : HandleFailure(result);
    }
}

#region Requests DTOs

public record ScheduleMaintenanceRequest(DateTime ScheduledDate, string Description);
public record RecordFuelConsumptionRequest(decimal Liters, decimal OdometerReading);
public record UpdateStatusRequest(Domain.Vehicles.Enums.VehicleStatus NewStatus);

#endregion