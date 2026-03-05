using Application.Features.Shipment.Commands.AddPackage;
using Application.Features.Shipment.Commands.AddRoutePoint;
using Application.Features.Shipment.Commands.AssignCarrier;
using Application.Features.Shipment.Commands.Cancel;
using Application.Features.Shipment.Commands.Create;
using Application.Features.Shipment.Commands.Dispatch;
using Application.Features.Shipment.Commands.MarkDelivered;
using Application.Features.Shipment.Commands.MarkDeliveryFailed;
using Application.Features.Shipment.Commands.RemovePackage;
using Application.Features.Shipment.Commands.UpdateDeliveryAddress;
using Application.Features.Shipment.Queries.GetAll;
using Application.Features.Shipment.Queries.GetById;
using Application.Features.Shipment.Queries.GetPackages;
using Application.Features.Shipments.ValueObjects;
using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShipmentsController : ApiController
{
    public ShipmentsController(ISender sender) : base(sender)
    {
    }

    // ─── Get all shipments ─────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetShipments(CancellationToken ct)
    {
        var query = new GetAllShipmentsQuery();
        var result = await _sender.Send(query, ct);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    // ─── Get shipment details ──────────────────────────
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetShipmentDetails([FromRoute] ShipmentId id, CancellationToken ct)
    {
        var query = new GetShipmentByIdQuery(id);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    // ─── Get Packages ──────────────────────────────────
    [HttpGet("{id:guid}/packages")]
    public async Task<IActionResult> GetShipmentPackages([FromRoute] ShipmentId id, CancellationToken ct)
    {
        var query = new GetShipmentPackagesQuery(id);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    // ─── Create new shipment ───────────────────────────
    [HttpPost]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    // ─── Add package ───────────────────────────────────
    [HttpPost("{id:guid}/packages")]
    public async Task<IActionResult> AddPackage([FromRoute] ShipmentId id, [FromBody] AddPackageCommand command, CancellationToken ct)
    {
        // ملحوظة: لو الـ Command محتاج الـ Id جواه، يفضل تساويه هنا قبل الإرسال
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    // ─── Assign carrier ────────────────────────────────
    [HttpPost("{id:guid}/assign-carrier")]
    public async Task<IActionResult> AssignCarrier([FromRoute] ShipmentId id, [FromBody] AssignCarrierCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Dispatch shipment ─────────────────────────────
    [HttpPost("{id:guid}/dispatch")]
    public async Task<IActionResult> DispatchShipment([FromRoute] ShipmentId id, CancellationToken ct)
    {
        var command = new DispatchCommand(id);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Mark as delivered ─────────────────────────────
    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> MarkDelivered([FromRoute] ShipmentId id, [FromBody] MarkShipmentDeliveredCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Mark delivery failed ──────────────────────────
    [HttpPost("{id:guid}/delivery-failed")]
    public async Task<IActionResult> MarkDeliveryFailed([FromRoute] ShipmentId id, [FromBody] MarkShipmentDeliveryFailedCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Cancel shipment ───────────────────────────────
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelShipment([FromRoute] ShipmentId id, [FromBody] CancelShipmentCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Update delivery address ───────────────────────
    [HttpPatch("{id:guid}/update-address")]
    public async Task<IActionResult> UpdateDeliveryAddress([FromRoute] ShipmentId id, [FromBody] UpdateShipmentDeliveryAddressCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Add route point ───────────────────────────────
    [HttpPost("{id:guid}/route-points")]
    public async Task<IActionResult> AddRoutePoint([FromRoute] ShipmentId id, [FromBody] AddRoutePointCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    // ─── Remove package ────────────────────────────────
    [HttpDelete("{id:guid}/packages/{packageId:guid}")]
    public async Task<IActionResult> RemovePackage([FromRoute] ShipmentId id, [FromRoute] PackageId packageId, CancellationToken ct)
    {
        var command = new RemovePackageCommand(id, packageId);
        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }
}