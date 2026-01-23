using Application.Features.Fleet.MaintenanceRecord.Commands.Create;
using Application.Features.Fleet.MaintenanceRecord.Commands.Delete;
using Application.Features.Fleet.MaintenanceRecord.Commands.Update;
using Application.Features.Fleet.MaintenanceRecord.DTOs;
using Application.Features.Fleet.MaintenanceRecord.Queries.GatAll;
using Application.Features.Fleet.MaintenanceRecord.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Fleet
{
    [Route("api/maintenance-records")]
    [ApiController]
    public class MaintenanceRecordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MaintenanceRecordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateMaintenanceRecord([FromBody] MaintenanceRecordDto maintenanceRecordDto)
        {
            var result = await _mediator.Send(new CreateMaintenanceRecordCommand(maintenanceRecordDto));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<MaintenanceRecordDto>>> GetAllMaintenanceRecords([FromQuery] GetAllMaintenanceRecordsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceRecordDto>> GetMaintenanceRecordById(Guid id)
        {
            var result = await _mediator.Send(new GetAllMaintenanceRecordsQuery());
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMaintenanceRecord(Guid id, [FromBody] UpdateMaintenanceRecordDto updateMaintenanceRecordDto)
        {
            await _mediator.Send(new UpdateMaintenanceRecordCommand(id, updateMaintenanceRecordDto));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaintenanceRecord(Guid id)
        {
            await _mediator.Send(new DeleteMaintenanceRecordCommand(id));
            return NoContent();
        }
    }
}