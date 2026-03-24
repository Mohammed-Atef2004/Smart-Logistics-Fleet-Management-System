using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Warehouse;
using Domain.Warehouse.Errors;
using Domain.Warehouse.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Warehouses.AddStorageLocation
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class AddStorageLocationCommandHandler
        : IRequestHandler<AddStorageLocationCommand, Result<AddStorageLocationResponse>>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddStorageLocationCommandHandler(
            IWarehouseRepository warehouseRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddStorageLocationResponse>> Handle(
            AddStorageLocationCommand command,
            CancellationToken cancellationToken)
        {
            var warehouseId = WarehouseId.From(command.WarehouseId);

            var warehouse = await _warehouseRepository.EntityQuery.FirstOrDefaultAsync(c=>c.Id==warehouseId, cancellationToken);
            if (warehouse is null)
                return Result<AddStorageLocationResponse>.Failure(WarehouseErrors.NotFound);

            var capacityResult = Capacity.Create(command.MaxSlots);
            if (capacityResult.IsFailure)
                return Result<AddStorageLocationResponse>.Failure(capacityResult.Error);

            var locationId = StorageLocationId.From(Guid.NewGuid());

            var result = warehouse.AddStorageLocation(locationId, command.Name, capacityResult.Value);
            if (result.IsFailure)
                return Result<AddStorageLocationResponse>.Failure(result.Error);

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result<AddStorageLocationResponse>.Success(
                new AddStorageLocationResponse(locationId.Value));
        }
    }
}