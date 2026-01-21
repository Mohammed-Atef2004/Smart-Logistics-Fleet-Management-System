using Application.Features.Fleet.Vehicle.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Fleet.Vehicle.Commands.Update;
using Application.Features.Fleet.Vehicle.Queries.GetById;
using Application.Features.Fleet.Vehicle.Queries.GetAll;
using Application.Features.Fleet.Vehicle.Commands.Delete;
using Application.Features.Fleet.Vehicle.DTOs;
using Application.Features.Fleet.Vehicle.Commands.Create;

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