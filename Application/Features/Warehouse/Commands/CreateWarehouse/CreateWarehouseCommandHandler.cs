using Domain.Interfaces.Repositories;
using Domain.SharedKernel;
using Domain.Warehouse;
using Domain.Warehouse.ValueObjects;
using MediatR;

namespace Application.Warehouses.CreateWarehouse
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class CreateWarehouseCommandHandler
        : IRequestHandler<CreateWarehouseCommand, Result<CreateWarehouseResponse>>
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWarehouseCommandHandler(
            IWarehouseRepository warehouseRepository,
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateWarehouseResponse>> Handle(
            CreateWarehouseCommand command,
            CancellationToken cancellationToken)
        {
            var addressResult = Address.Create(
                command.Street,
                command.City,
                command.Country,
                command.ZipCode);

            if (addressResult.IsFailure)
                return Result<CreateWarehouseResponse>.Failure(addressResult.Error);

            var warehouseId = WarehouseId.From(Guid.NewGuid());

            var warehouseResult = Warehouse.Create(warehouseId, command.Name, addressResult.Value);
            if (warehouseResult.IsFailure)
                return Result<CreateWarehouseResponse>.Failure(warehouseResult.Error);

            await _warehouseRepository.AddAsync(warehouseResult.Value);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result<CreateWarehouseResponse>.Success(
                new CreateWarehouseResponse(warehouseId.Id));
        }
    }
}