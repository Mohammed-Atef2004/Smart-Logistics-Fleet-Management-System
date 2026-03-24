using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Warehouse;
using Domain.Warehouse.Errors;
using Domain.Warehouse.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Warehouses.DeactivateWarehouse
{

    public sealed class DeactivateWarehouseCommandHandler
        : IRequestHandler<DeactivateWarehouseCommand, Result>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateWarehouseCommandHandler(
            IWarehouseRepository warehouseRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            DeactivateWarehouseCommand command,
            CancellationToken cancellationToken)
        {
            var warehouseId = WarehouseId.From(command.WarehouseId);

            var warehouse = await _warehouseRepository.EntityQuery.FirstOrDefaultAsync(c => c.Id == warehouseId, cancellationToken);
            if (warehouse is null)
                return Result.Failure(WarehouseErrors.NotFound);

            var result = warehouse.Deactivate();
            if (result.IsFailure)
                return Result.Failure(result.Error);

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}