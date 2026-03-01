using Domain.SharedKernel;
using Domain.Shipments.Errors;
using Domain.Shipments.Rules;
using Domain.Shipments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments
{
    public sealed class Package
    {
        // ─── Properties ──────────────────────────────────────────
        public PackageId Id { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public Weight Weight { get; private set; } = default!;
        public Dimensions Dimensions { get; private set; } = default!;
        public string? ContentCategory { get; private set; }
        public bool IsFragile { get; private set; }
        public bool RequiresRefrigeration { get; private set; }
        public decimal DeclaredValue { get; private set; }
        public string Currency { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }

        // ─── EF Core ─────────────────────────────────────────────
        private Package() { }

        private Package(
            PackageId id,
            string description,
            Weight weight,
            Dimensions dimensions,
            string? contentCategory,
            bool isFragile,
            bool requiresRefrigeration,
            decimal declaredValue,
            string currency)
        {
            Id = id;
            Description = description;
            Weight = weight;
            Dimensions = dimensions;
            ContentCategory = contentCategory;
            IsFragile = isFragile;
            RequiresRefrigeration = requiresRefrigeration;
            DeclaredValue = declaredValue;
            Currency = currency;
            CreatedAt = DateTime.UtcNow;
        }

        // ─── Factory ─────────────────────────────────────────────

        internal static Result<Package> Create(
            string description,
            Weight weight,
            Dimensions dimensions,
            string? contentCategory = null,
            bool isFragile = false,
            bool requiresRefrigeration = false,
            decimal declaredValue = 0,
            string currency = "USD")
        {
            if (string.IsNullOrWhiteSpace(description))
                return Result<Package>.Failure(PackageErrors.EmptyDescription);

            if (string.IsNullOrWhiteSpace(currency))
                return Result<Package>.Failure(PackageErrors.EmptyCurrency);

            var declaredValueRule = new DeclaredValueMustNotBeNegativeRule(declaredValue);
            if (declaredValueRule.IsBroken())
                return Result<Package>.Failure(declaredValueRule.Error);

                var package=new Package(
                PackageId.New(),
                description.Trim(),
                weight,
                dimensions,
                contentCategory?.Trim(),
                isFragile,
                requiresRefrigeration,
                declaredValue,
                currency.ToUpperInvariant());
            return Result<Package>.Success(package);
        }

        // ─── Behavior ─────────────────────────────────────────────

        internal Result UpdateDetails(string description, Weight weight, Dimensions dimensions)
        {
            if (string.IsNullOrWhiteSpace(description))
                return Result<Package>.Failure(PackageErrors.EmptyDescription);

            Description = description.Trim();
            Weight = weight;
            Dimensions = dimensions;

            return Result.Success();
        }

        internal void MarkAsFragile() => IsFragile = true;
        internal void MarkAsRequiresRefrigeration() => RequiresRefrigeration = true;

        // ─── Computed ─────────────────────────────────────────────

        public decimal BillableWeightKg =>
            Math.Max(Weight.ToKilograms(), Dimensions.VolumetricWeightKg);
    }
}
