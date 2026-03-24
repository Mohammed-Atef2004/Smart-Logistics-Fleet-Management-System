using Application.Inventory.AddStock;
using Application.Inventory.AdjustReorderThreshold;
using Application.Inventory.CreateInventoryItem;
using Application.Inventory.DeactivateInventoryItem;
using Application.Inventory.GetAllInventoryItems;
using Application.Inventory.GetInventoryItemById;
using Application.Inventory.RemoveStock;
using Application.Inventory.UpdateProductInfo;
using Application.Inventory.UpdateWeight;
using Domain.Inventory.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/inventory")]
    public sealed class InventoryController : ApiController
    {
        public InventoryController(ISender sender) : base(sender) { }

        // ──────────────────────────── Queries ────────────────────────────

        /// <summary>Get all inventory items</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<GetAllInventoryItemsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllInventoryItemsQuery(activeOnly), cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : Ok(result.Value);
        }

        /// <summary>Get inventory item by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetInventoryItemByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            InventoryItemId id,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetInventoryItemByIdQuery(id), cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : Ok(result.Value);
        }

        // ──────────────────────────── Commands ───────────────────────────

        /// <summary>Create a new inventory item</summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreateInventoryItemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateInventoryItemCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Value.InventoryItemId },
                    result.Value);
        }

        /// <summary>Add stock units to an inventory item</summary>
        [HttpPost("{id:guid}/stock/add")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddStock(
            Guid id,
            [FromBody] StockUnitsRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new AddStockCommand(id, request.Units),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Remove stock units from an inventory item</summary>
        [HttpPost("{id:guid}/stock/remove")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveStock(
            Guid id,
            [FromBody] StockUnitsRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new RemoveStockCommand(id, request.Units),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Adjust the reorder threshold of an inventory item</summary>
        [HttpPatch("{id:guid}/reorder-threshold")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdjustReorderThreshold(
            Guid id,
            [FromBody] AdjustReorderThresholdRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new AdjustReorderThresholdCommand(id, request.NewThreshold),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Update product info of an inventory item</summary>
        [HttpPut("{id:guid}/product-info")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProductInfo(
            Guid id,
            [FromBody] UpdateProductInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateProductInfoCommand(
                id,
                request.Sku,
                request.Name,
                request.Description);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Update weight of an inventory item</summary>
        [HttpPatch("{id:guid}/weight")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWeight(
            Guid id,
            [FromBody] UpdateWeightRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateWeightCommand(id, request.WeightValue, request.WeightUnit);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Deactivate an inventory item</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new DeactivateInventoryItemCommand(id),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }
    }

    // ──────────────────────────── Request Bodies ──────────────────────────

    public sealed record StockUnitsRequest(int Units);
    public sealed record AdjustReorderThresholdRequest(int NewThreshold);
    public sealed record UpdateProductInfoRequest(string Sku, string Name, string? Description);
    public sealed record UpdateWeightRequest(decimal WeightValue, string WeightUnit);
}