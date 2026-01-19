using Application.Features.Vehicle.Commands.CreateVehicle;
using Application.Features.Vehicle.Queries;
using Application.Features.Vehicles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Vehicle.Commands.UpdateVehicle;
using Application.Features.Vehicle.Queries.GetVehicleDetails;
using Application.Features.Vehicle.Queries.GetAll;
using Application.Features.Vehicle.Commands.Delete;

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

}