using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Application.Features.Fleet.Vehicle.Commands.AddMaintenanceRecord;
using Application.Features.Fleet.Vehicle.Commands.CompleteMaintenance;
using Application.Features.Fleet.Vehicle.Commands.Create;
using Application.Features.Fleet.Vehicle.Commands.Delete;
using Application.Features.Fleet.Vehicle.Commands.Update;
using Application.Features.Fleet.Vehicle.Commands.UpdateMileage;
using Application.Features.Fleet.Vehicle.DTOs;
using Application.Features.Fleet.Vehicle.Queries;
using Application.Features.Fleet.Vehicle.Queries.GetAll;
using Application.Features.Fleet.Vehicle.Queries.GetAllMaintenanceRecordForVehicle;
using Application.Features.Fleet.Vehicle.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    public VehiclesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("Create")]
    public async Task<ActionResult<Guid>> Create(CreateVehicleCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> Get(Guid id)
    {
        return await _mediator.Send(new GetVehicleDetailsQuery(id));
    }
    [HttpPost]
    [Route("update/{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateVechicleDto updateVechicleDto)
    {
        await _mediator.Send(new UpdateVehicleCommand(id, updateVechicleDto));
        return NoContent();
    }
    [HttpGet("All")]
    public async Task<ActionResult<List<VehicleDto>>> GetAll()
    {
        return await _mediator.Send(new GetAllVehiclesQuery());
    }
    [HttpPost("Delete/{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteVehicleCommand(id));
        return NoContent();
    }
    [HttpGet("MaintenanceRecordsPerVehicle/{id}")]
    public async Task<ActionResult<List<MaintenanceRecordDto>>> GetAllMaintenanceRecordsForVehicle(Guid id)
    {
        return await _mediator.Send(new GetAllMaintenanceRecordForVehicleQuery(id));
    }


    [HttpPost("AddMaintenance/{id}")]
    public async Task<IActionResult> AddMaintenance([FromBody] MaintenanceRecordDto dto)
    {
        var command = new AddMaintenanceRecordCommand(dto);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("complete-maintenance/{id}")]
    public async Task<IActionResult> CompleteMaintenance(Guid id)
    {
        await _mediator.Send(new CompleteMaintenanceCommand(id));
        return Ok(new { Message = "Vehicle is now Available" });
    }
    [HttpPost("UpdateMileage/{id}")]
    public async Task<IActionResult> UpdateMileage(Guid id, [FromBody] int mileage)
    {
        var command = new UpdateMileageCommand(id, mileage);
        await _mediator.Send(command);
        return Ok();
    }
}