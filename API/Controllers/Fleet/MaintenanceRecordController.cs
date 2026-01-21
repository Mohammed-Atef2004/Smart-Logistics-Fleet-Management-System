using Application.Features.Fleet.MaintenanceRecord.Commands.Create;
using Application.Features.Fleet.MaintenanceRecord.Commands.Delete;
using Application.Features.Fleet.MaintenanceRecord.Commands.Update;
using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Application.Features.Fleet.MaintenanceRecord.Queries.GatAll;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Fleet
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceRecordController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MaintenanceRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create")]
        public async Task<ActionResult<Guid>> CreateMaintenanceRecord([FromBody] MaintenanceRecordDto maintenanceRecordDto)
        {
            var result = await _mediator.Send(new CreateMaintenanceRecordCommand(maintenanceRecordDto));
            return Ok(result);
        }
        [HttpGet("GatAll")]
        public async Task<ActionResult<List<MaintenanceRecordDto>>> GetAllMaintenanceRecords()
        {
            var result = await _mediator.Send(new GetAllMaintenanceRecordsQuery());
            return Ok(result);
        }
        [HttpGet("GetById")]
        public async Task<ActionResult<MaintenanceRecordDto>> GetMaintenanceRecordById([FromQuery] Guid id)
        {
            var result = await _mediator.Send(new GetAllMaintenanceRecordsQuery());
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPut("Update")]
        public async Task<ActionResult> UpdateMaintenanceRecord([FromQuery] Guid id, [FromBody] UpdateMaintenanceRecordDto updateMaintenanceRecordDto)
        {
            await _mediator.Send(new UpdateMaintenanceRecordCommand(id, updateMaintenanceRecordDto));
            return NoContent();
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteMaintenanceRecord([FromQuery] Guid id)
        {
            await _mediator.Send(new DeleteMaintenanceRecordCommand(id));
            return NoContent();
        }
    }
}
