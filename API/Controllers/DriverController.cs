using Application.Features.Driver.Commands.Create;
using Application.Features.Driver.Dtos;
using Application.Features.Driver.Queries.GetById;
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
    }
}
