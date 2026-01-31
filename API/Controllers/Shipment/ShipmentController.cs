using Application.Features.Shipment.Shipment.Commands.AddPackage;
using Application.Features.Shipment.Shipment.Commands.AssignVehicle;
using Application.Features.Shipment.Shipment.Commands.CancelShipment;
using Application.Features.Shipment.Shipment.Commands.CompleteDelivery;
using Application.Features.Shipment.Shipment.Commands.Create;
using Application.Features.Shipment.Shipment.Commands.OutOfDelivery;
using Application.Features.Shipment.Shipment.Commands.RecordLocation;
using Application.Features.Shipment.Shipment.Commands.RemovePackage;
using Application.Features.Shipment.Shipment.Commands.StartJourney;
using Application.Features.Shipment.Shipment.DTOs;
using Application.Features.Shipment.Shipment.Queries.GetDelayed;
using Application.Features.Shipment.Shipment.Queries.GetShipmentById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Shipment
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ShipmentsController(IMediator mediator) => _mediator = mediator;

        // ============================================
        // COMMANDS (Write Operations)
        // ============================================

        [HttpPost]
        public async Task<IActionResult> Create(CreateShipmentCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPost("{id}/packages")]
        public async Task<IActionResult> AddPackage(Guid id, ShipmentPackageDto request)
        {
            await _mediator.Send(new AddPackageCommand(id, request));
            return NoContent();
        }

        [HttpDelete("{id}/packages/{packageId}")]
        public async Task<IActionResult> RemovePackage(Guid id, Guid packageId)
        {
            await _mediator.Send(new RemovePackageCommand(id, packageId));
            return NoContent();
        }

        [HttpPut("{id}/assign-vehicle")]
        public async Task<IActionResult> AssignVehicle(Guid id, [FromBody] Guid vehicleId)
        {
            await _mediator.Send(new AssignVehicleCommand(id, vehicleId));
            return NoContent();
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartJourney(Guid id)
        {
            await _mediator.Send(new StartJourneyCommand(id));
            return NoContent();
        }

     
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            await _mediator.Send(new CompleteDeliveryCommand(id));
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] string reason)
        {
            await _mediator.Send(new CancelShipmentCommand(id, reason));
            return NoContent();
        }

        [HttpPost("{id}/track")]
        public async Task<IActionResult> RecordLocation(Guid id, RecordLocationCommand request)
        {
            await _mediator.Send(new RecordLocationCommand(id, request.location, request.notes));
            return NoContent();
        }

        // ============================================
        // QUERIES (Read Operations)
        // ============================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetShipmentByIdQuery(id));
            return Ok(result);
        }


        [HttpGet("delayed")]
        public async Task<IActionResult> GetDelayed()
        {
            var result = await _mediator.Send(new GetDelayedShipmentsQuery());
            return Ok(result);
        }

       
    }
}
