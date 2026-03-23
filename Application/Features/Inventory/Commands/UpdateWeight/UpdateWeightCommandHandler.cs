using Domain.Interfaces.Repositories;
using Domain.Inventory;
using Domain.Inventory.Errors;
using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Inventory.UpdateWeight
{
    // ──────────────────────────── Handler ────────────────────────────

    public sealed class UpdateWeightCommandHandler : IRequestHandler<UpdateWeightCommand, Result>
    {
        private readonly IInventoryItemRepository _inventoryItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWeightCommandHandler(
            IInventoryItemRepository inventoryItemRepository,
            IUnitOfWork unitOfWork)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateWeightCommand command, CancellationToken cancellationToken)
        {
            var itemId = InventoryItemId.From(command.InventoryItemId);

            var item = await _inventoryItemRepository.EntityQuery.FirstOrDefaultAsync(c => c.Id == itemId, cancellationToken);
            if (item is null)
                return Result.Failure(InventoryErrors.NotFound);

            var unit = Enum.Parse<WeightUnit>(command.WeightUnit, ignoreCase: true);
            var weightResult = Weight.Create(command.WeightValue, unit);
            if (weightResult.IsFailure)
                return Result.Failure(weightResult.Error);

            var result = item.UpdateWeight(weightResult.Value);
            if (result.IsFailure)
                return Result.Failure(result.Error);

            _inventoryItemRepository.Update(item);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}