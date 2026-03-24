using Application.Warehouses.AddStorageLocation;
using Application.Warehouses.AssignItemToLocation;
using Application.Warehouses.CreateWarehouse;
using Application.Warehouses.DeactivateWarehouse;
using Application.Warehouses.GetAllWarehouses;
using Application.Warehouses.GetWarehouseById;
using Application.Warehouses.RemoveStorageLocation;
using Application.Warehouses.UnassignItemFromLocation;
using Application.Warehouses.UpdateAddress;
using Domain.Warehouse.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/warehouses")]
    public sealed class WarehousesController : ApiController
    {
        public WarehousesController(ISender sender) : base(sender) { }


        /// <summary>Get all warehouses</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<GetAllWarehousesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllWarehousesQuery(activeOnly), cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : Ok(result.Value);
        }

        /// <summary>Get warehouse by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetWarehouseByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            WarehouseId id,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetWarehouseByIdQuery(id), cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : Ok(result.Value);
        }

        // ──────────────────────────── Commands ───────────────────────────

        /// <summary>Create a new warehouse</summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreateWarehouseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateWarehouseCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Value.WarehouseId },
                    result.Value);
        }

        /// <summary>Add a storage location to a warehouse</summary>
        [HttpPost("{warehouseId:guid}/storage-locations")]
        [ProducesResponseType(typeof(AddStorageLocationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddStorageLocation(
            Guid warehouseId,
            [FromBody] AddStorageLocationRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new AddStorageLocationCommand(warehouseId, request.Name, request.MaxSlots);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : CreatedAtAction(
                    nameof(GetById),
                    new { id = warehouseId },
                    result.Value);
        }

        /// <summary>Remove a storage location from a warehouse</summary>
        [HttpDelete("{warehouseId:guid}/storage-locations/{locationId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveStorageLocation(
            Guid warehouseId,
            Guid locationId,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new RemoveStorageLocationCommand(warehouseId, locationId),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Assign an inventory item to a storage location</summary>
        [HttpPost("{warehouseId:guid}/storage-locations/{locationId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignItem(
            Guid warehouseId,
            Guid locationId,
            Guid itemId,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new AssignItemToLocationCommand(warehouseId, locationId, itemId),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Unassign an inventory item from a storage location</summary>
        [HttpDelete("{warehouseId:guid}/storage-locations/{locationId:guid}/items/{itemId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnassignItem(
            Guid warehouseId,
            Guid locationId,
            Guid itemId,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new UnassignItemFromLocationCommand(warehouseId, locationId, itemId),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Update warehouse address</summary>
        [HttpPut("{warehouseId:guid}/address")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAddress(
            Guid warehouseId,
            [FromBody] UpdateAddressRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateWarehouseAddressCommand(
                warehouseId,
                request.Street,
                request.City,
                request.Country,
                request.ZipCode);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }

        /// <summary>Deactivate a warehouse</summary>
        [HttpDelete("{warehouseId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(
            Guid warehouseId,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new DeactivateWarehouseCommand(warehouseId),
                cancellationToken);

            return result.IsFailure
                ? HandleFailure(result)
                : NoContent();
        }
    }

    // ──────────────────────────── Request Bodies ──────────────────────────

    public sealed record AddStorageLocationRequest(string Name, int MaxSlots);
    public sealed record UpdateAddressRequest(string Street, string City, string Country, string ZipCode);
}