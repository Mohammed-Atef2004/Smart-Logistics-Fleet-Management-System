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
using Domain.Shipments.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Shipment endpoints. The shipment id always comes from the route and is
/// injected into the command - the old actions bound <c>ShipmentId</c>
/// straight from the route, which the model binder cannot construct, and
/// ignored the route id for most commands.
/// </summary>
[ApiController]
[Route("api/shipments")]
public sealed class ShipmentsController : ApiController
{
    public ShipmentsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ShipmentStatus? status,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllShipmentsQuery(status), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> GetById(Guid shipmentId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetShipmentByIdQuery(shipmentId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{shipmentId:guid}/packages")]
    public async Task<IActionResult> GetPackages(Guid shipmentId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetShipmentPackagesQuery(shipmentId), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateShipmentCommand command,
        CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { shipmentId = result.Value }, result.Value);
    }

    [HttpPost("{shipmentId:guid}/packages")]
    public async Task<IActionResult> AddPackage(
        Guid shipmentId,
        [FromBody] AddPackageRequest request,
        CancellationToken ct)
    {
        var command = new AddPackageCommand(
            shipmentId,
            request.Description,
            request.WeightValue,
            request.WeightUnit,
            request.Length,
            request.Width,
            request.Height,
            request.DimensionUnit,
            request.Category,
            request.IsFragile,
            request.RequiresRefrigeration,
            request.DeclaredValue,
            request.Currency);

        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(nameof(GetById), new { shipmentId }, result.Value);
    }

    [HttpPost("{shipmentId:guid}/assign-carrier")]
    public async Task<IActionResult> AssignCarrier(
        Guid shipmentId,
        [FromBody] AssignCarrierRequest request,
        CancellationToken ct)
    {
        var command = new AssignCarrierCommand(
            shipmentId, request.CarrierName, request.EstimatedDeliveryDate);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shipmentId:guid}/dispatch")]
    public async Task<IActionResult> Dispatch(Guid shipmentId, CancellationToken ct)
    {
        var result = await _sender.Send(new DispatchCommand(shipmentId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shipmentId:guid}/deliver")]
    public async Task<IActionResult> MarkDelivered(
        Guid shipmentId,
        [FromBody] MarkDeliveredRequest request,
        CancellationToken ct)
    {
        var command = new MarkShipmentDeliveredCommand(
            shipmentId, request.DeliveredAt, request.ReceivedBy);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shipmentId:guid}/delivery-failed")]
    public async Task<IActionResult> MarkDeliveryFailed(
        Guid shipmentId,
        [FromBody] MarkDeliveryFailedRequest request,
        CancellationToken ct)
    {
        var command = new MarkShipmentDeliveryFailedCommand(shipmentId, request.Reason);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shipmentId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid shipmentId,
        [FromBody] CancelShipmentRequest request,
        CancellationToken ct)
    {
        var command = new CancelShipmentCommand(shipmentId, request.Reason, request.CancelledBy);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPatch("{shipmentId:guid}/address")]
    public async Task<IActionResult> UpdateDeliveryAddress(
        Guid shipmentId,
        [FromBody] UpdateShipmentAddressRequest request,
        CancellationToken ct)
    {
        var command = new UpdateShipmentDeliveryAddressCommand(
            shipmentId,
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.ApartmentUnit);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("{shipmentId:guid}/route-points")]
    public async Task<IActionResult> AddRoutePoint(
        Guid shipmentId,
        [FromBody] AddRoutePointRequest request,
        CancellationToken ct)
    {
        var command = new AddRoutePointCommand(
            shipmentId,
            request.Location,
            request.Description,
            request.ArrivedAt,
            request.Type);

        var result = await _sender.Send(command, ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpDelete("{shipmentId:guid}/packages/{packageId:guid}")]
    public async Task<IActionResult> RemovePackage(
        Guid shipmentId,
        Guid packageId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new RemovePackageCommand(shipmentId, packageId), ct);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }
}

#region Request DTOs

public sealed record AddPackageRequest(
    string Description,
    decimal WeightValue,
    WeightUnit WeightUnit,
    decimal Length,
    decimal Width,
    decimal Height,
    DimensionUnit DimensionUnit,
    string? Category = null,
    bool IsFragile = false,
    bool RequiresRefrigeration = false,
    decimal DeclaredValue = 0,
    string Currency = "USD");

public sealed record AssignCarrierRequest(
    string CarrierName,
    DateTime? EstimatedDeliveryDate = null);

public sealed record MarkDeliveredRequest(
    DateTime DeliveredAt,
    string? ReceivedBy = null);

public sealed record MarkDeliveryFailedRequest(string Reason);

public sealed record CancelShipmentRequest(string Reason, string CancelledBy);

public sealed record UpdateShipmentAddressRequest(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? ApartmentUnit = null);

public sealed record AddRoutePointRequest(
    string Location,
    string Description,
    DateTime ArrivedAt,
    RoutePointType Type);

#endregion
