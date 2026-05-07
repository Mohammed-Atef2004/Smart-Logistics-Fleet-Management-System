using Application.Features.Invoices.Commands.AddItem;
using Application.Features.Invoices.Commands.Cancel;
using Application.Features.Invoices.Commands.Create;
using Application.Features.Invoices.Commands.Issue;
using Application.Features.Invoices.Queries.GetAll;
using Application.Features.Invoices.Queries.GetById;
using Domain.Invoices.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllInvoicesQuery();

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{invoiceId}")]
        public async Task<IActionResult> GetById(
            InvoiceId invoiceId,
            CancellationToken cancellationToken)
        {
            var query = new GetInvoiceByIdQuery(invoiceId);

            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CancellationToken cancellationToken)
        {
            var command = new CreateInvoiceCommand();

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(
                nameof(GetById),
                new { invoiceId = result.Value },
                result.Value);
        }

        [HttpPost("{invoiceId}/add-item")]
        public async Task<IActionResult> AddItem(
            [FromHeader]InvoiceId invoiceId,
            [FromBody] AddInvoiceItemRequest request,
            CancellationToken cancellationToken)
        {
            var command = new AddInvoiceItemCommand(
                invoiceId,
                request.Description,
                request.Price,
                request.Quantity);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpPost("{invoiceId}/issue")]
        public async Task<IActionResult> Issue(
            InvoiceId invoiceId,
            CancellationToken cancellationToken)
        {
            var command = new IssueInvoiceCommand(invoiceId);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpPost("{invoiceId}/cancel")]
        public async Task<IActionResult> Cancel(
            InvoiceId invoiceId,
            CancellationToken cancellationToken)
        {
            var command = new CancelInvoiceCommand(invoiceId);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result);
        }
    }


    public sealed class AddInvoiceItemRequest
    {
        public string Description { get; set; } = default!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}