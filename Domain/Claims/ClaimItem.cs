using Domain.Claims.Errors;
using Domain.Claims.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Claims;

public sealed class ClaimItem 
{
    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public ClaimAmount UnitValue { get; private set; }
    public int Quantity { get; private set; }
    public ClaimAmount TotalValue => ClaimAmount.Of(UnitValue.Value * Quantity, UnitValue.Currency);

    private ClaimItem() { } // EF Core

    internal static Result<ClaimItem> Create(string description, ClaimAmount unitValue, int quantity)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result<ClaimItem>.Failure(ClaimErrors.EmptyItemDescription);

        if (quantity <= 0)
            return Result<ClaimItem>.Failure(ClaimErrors.InvalidItemQuantity);

        return Result<ClaimItem>.Success(new ClaimItem
        {
            Id = Guid.NewGuid(),
            Description = description,
            UnitValue = unitValue,
            Quantity = quantity
        });
    }
}
