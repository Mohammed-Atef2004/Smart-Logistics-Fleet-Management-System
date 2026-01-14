using Application.Features.Vehicle.Commands.CreateVehicle;
using Application.Features.Vehicle.Queries;
using Application.Features.Vehicles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    public VehiclesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateVehicleCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> Get(Guid id)
    {
        return await _mediator.Send(new GetVehicleDetailsQuery(id));
    }
  
}