using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Warehouse;
using Domain.Warehouse.Errors;
using Domain.Warehouse.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Warehouses.UpdateAddress
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class UpdateWarehouseAddressCommandHandler
        : IRequestHandler<UpdateWarehouseAddressCommand, Result>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWarehouseAddressCommandHandler(
            IWarehouseRepository warehouseRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            UpdateWarehouseAddressCommand command,
            CancellationToken cancellationToken)
        {
            var warehouseId = WarehouseId.From(command.WarehouseId);

            var warehouse = await _warehouseRepository.EntityQuery.FirstOrDefaultAsync(c=>c.Id==warehouseId, cancellationToken);
            if (warehouse is null)
                return Result.Failure(WarehouseErrors.NotFound);

            var addressResult = Address.Create(
                command.Street,
                command.City,
                command.Country,
                command.ZipCode);

            if (addressResult.IsFailure)
                return Result.Failure(addressResult.Error);

            var result = warehouse.UpdateAddress(addressResult.Value);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            _warehouseRepository.Update(warehouse);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}