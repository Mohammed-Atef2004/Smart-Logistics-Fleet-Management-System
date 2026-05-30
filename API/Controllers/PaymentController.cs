using Application.Features.Payments.Queries;
using Application.Payments.Commands.ProcessPayment;
using Application.Payments.Commands.RefundPayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SLFMS.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    // =========================================================
    // PROCESS PAYMENT
    // =========================================================
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment(
        ProcessPaymentCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Created($"/api/payments/{result.Value}", new { id = result.Value })
            : BadRequest(new
            {
                error = result.Error.Code,
                message = result.Error.Message
            });
    }

    // =========================================================
    // REFUND PAYMENT
    // =========================================================
    [HttpPut("{id:guid}/refund")]
    public async Task<IActionResult> RefundPayment(
        Guid id,
        CancellationToken ct)
    {
        var result = await _sender.Send(new RefundPaymentCommand(id), ct);

        return result.IsSuccess
            ? NoContent()
            : BadRequest(new
            {
                error = result.Error.Code,
                message = result.Error.Message
            });
    }

    // =========================================================
    // GET PAYMENT DETAILS
    // =========================================================
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentDetails(
        Guid id,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetPaymentDetailsQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new
            {
                error = result.Error.Code,
                message = result.Error.Message
            });
    }

    // =========================================================
    // PAYMENT HISTORY
    // =========================================================
    [HttpGet("history")]
    public async Task<IActionResult> GetPaymentHistory(
        [FromQuery] Guid? invoiceId,
        [FromQuery] string? status,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GetPaymentHistoryQuery(invoiceId, status), ct);

        return Ok(result);
    }
}