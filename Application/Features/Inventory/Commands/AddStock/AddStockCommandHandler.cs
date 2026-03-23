using Domain.Interfaces.Repositories;
using Domain.Inventory;
using Domain.Inventory.Errors;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.AddStock
{

    public sealed class AddStockCommandHandler : IRequestHandler<AddStockCommand, Result>
    {
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddStockCommandHandler(
            IInventoryItemRepository inventoryItemRepository,
            IUnitOfWork unitOfWork)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AddStockCommand command, CancellationToken cancellationToken)
        {
            var itemId = InventoryItemId.From(command.InventoryItemId);

            var item = await _inventoryItemRepository.EntityQuery.FirstOrDefaultAsync(c=>c.Id==itemId, cancellationToken);
            if (item is null)
                return Result.Failure(InventoryErrors.NotFound);

            var result = item.AddStock(command.Units);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            _inventoryItemRepository.Update(item);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}