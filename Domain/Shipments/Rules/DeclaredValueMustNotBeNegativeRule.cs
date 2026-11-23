using Domain.SharedKernel;
using Domain.Shipments.Errors;

namespace Domain.Shipments.Rules;

/// <summary>A declared value below zero is meaningless.</summary>
public sealed class DeclaredValueMustNotBeNegativeRule : IBusinessRule
{
    private readonly decimal _declaredValue;

    public DeclaredValueMustNotBeNegativeRule(decimal declaredValue) => _declaredValue = declaredValue;

    public Error Error => PackageErrors.InvalidDeclaredValue;

    public bool IsBroken() => _declaredValue < 0;
}
