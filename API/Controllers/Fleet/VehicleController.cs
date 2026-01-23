using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Application.Features.Fleet.Vehicle.Commands.AddMaintenanceRecord;
using Application.Features.Fleet.Vehicle.Commands.CompleteMaintenance;
using Application.Features.Fleet.Vehicle.Commands.Create;
using Application.Features.Fleet.Vehicle.Commands.Delete;
using Application.Features.Fleet.Vehicle.Commands.Update;
using Application.Features.Fleet.Vehicle.Commands.UpdateMileage;
using Application.Features.Fleet.Vehicle.DTOs;
using Application.Features.Fleet.Vehicle.Queries;
using Application.Features.Fleet.Vehicle.Queries.Alert;
using Application.Features.Fleet.Vehicle.Queries.GetAll;
using Application.Features.Fleet.Vehicle.Queries.GetAllMaintenanceRecordForVehicle;
using Application.Features.Fleet.Vehicle.Queries.GetById;
using Application.Features.Fleet.Vehicle.Queries.Summary;
using Domain.Fleet.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/vehicles")] 
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    public VehiclesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateVehicleCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id); // الـ 201 Created هي الصح هنا
    }

    [HttpGet]
    public async Task<ActionResult<List<VehicleDto>>> GetAll([FromQuery] GetAllVehiclesQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VehicleDto>> Get(Guid id)
    {
        return Ok(await _mediator.Send(new GetVehicleDetailsQuery(id)));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateVechicleDto dto)
    {
        await _mediator.Send(new UpdateVehicleCommand(id, dto));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteVehicleCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}/maintenance-records")]
    public async Task<ActionResult<List<MaintenanceRecordDto>>> GetMaintenanceRecords(Guid id)
    {
        return Ok(await _mediator.Send(new GetAllMaintenanceRecordForVehicleQuery(id)));
    }

    [HttpPost("{id:guid}/maintenance")]
    public async Task<IActionResult> AddMaintenance([FromBody] MaintenanceRecordDto dto)
    {
        await _mediator.Send(new AddMaintenanceRecordCommand(dto));
        return Ok();
    }

    [HttpPost("{id:guid}/complete-maintenance")]
    public async Task<IActionResult> CompleteMaintenance(Guid id)
    {
        await _mediator.Send(new CompleteMaintenanceCommand(id));
        return Ok(new { Message = "Vehicle is now Available" });
    }

    [HttpPatch("{id:guid}/mileage")] // استخدام PATCH لأننا بنحدث جزء فقط
    public async Task<IActionResult> UpdateMileage(Guid id, [FromBody] int mileage)
    {
        await _mediator.Send(new UpdateMileageCommand(id, mileage));
        return Ok();
    }

    [HttpGet("summary")]
    public async Task<ActionResult<FleetSummaryDto>> GetSummary()
        => Ok(await _mediator.Send(new GetFleetSummaryQuery()));

    [HttpGet("alerts")]
    public async Task<ActionResult<List<MaintenanceAlertDto>>> GetAlerts()
        => Ok(await _mediator.Send(new GetMaintenanceAlertsQuery()));
}