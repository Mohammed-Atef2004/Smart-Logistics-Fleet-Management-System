using Application.Features.Fleet.Driver.Commands.Create;
using Application.Features.Fleet.Driver.Commands.Update;
using Application.Features.Fleet.Driver.Queries.GetAll;
using Application.Features.Fleet.Driver.Queries.GetById;
using Application.Features.Fleet.Driver.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Fleet.Driver.Commands.AssignToVehicle;
using Application.Features.Fleet.Driver.Commands.Activate;
using Application.Features.Fleet.Driver.Commands.Deactivate;

namespace API.Controllers
{
    [Route("api/drivers")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DriverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(DriverDto driverDto)
        {
            var command = new CreateDriverCommand(driverDto);
            Guid result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDto>> GetById(Guid id)
        {
            return await _mediator.Send(new GetDriverDetailsQuery(id));
        }

        [HttpGet]
        public async Task<ActionResult<List<DriverDto>>> GetAll()
        {
            return await _mediator.Send(new GetAllDriversQuery());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Update(Guid id, DriverDto driverDto)
        {
            await _mediator.Send(new UpdateDriversCommand(id, driverDto));
            return Unit.Value;
        }

        [HttpPost("{driverId}/assignments")]
        public async Task<ActionResult<Unit>> AssignToVehicle(Guid driverId, [FromBody] Guid vehicleId)
        {
            await _mediator.Send(new DriverAssignToVehicleCommand(driverId, vehicleId));
            return Unit.Value;
        }

        [HttpPatch("{id}/activate")]
        public async Task<ActionResult<Unit>> Activate(Guid id)
        {
            await _mediator.Send(new ActivateDriverCommand(id));
            return Unit.Value;
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult<Unit>> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateDriverCommand(id));
            return Unit.Value;
        }
    }
}