using Application.Features.Fleet.Driver.Commands.Create;
using Application.Features.Fleet.Driver.Commands.Update;
using Application.Features.Fleet.Driver.Queries.GetAll;
using Application.Features.Fleet.Driver.Queries.GetById;
using Application.Features.Fleet.Driver.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DriverController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Guid>> Create(DriverDto driverDto)
        {
            var command = new CreateDriverCommand(driverDto);
            Guid result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("GetById")]
        public async Task<ActionResult<DriverDto>> GetById(Guid id)
        {
            return await _mediator.Send(new GetDriverDetailsQuery(id));
        }
        [HttpGet("GetAll")]
        public async Task<List<DriverDto>> GetAll()
        {
            return await _mediator.Send(new GetAllDriversQuery());
        }
        [HttpPost("Update")]
        public async Task<Unit> Update(Guid guid,DriverDto driverDto)
        {
            await _mediator.Send(new UpdateDriversCommand(guid,driverDto));
            return Unit.Value;
        }
        
    }
}
