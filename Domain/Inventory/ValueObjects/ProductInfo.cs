using Domain.Common.Domain.Common;
using Domain.Inventory.Errors;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.ValueObjects
{
    public sealed class ProductInfo : ValueObject
    {
        public string Sku { get; }
        public string Name { get;  }
        public string? Description { get;}

        private ProductInfo() { }

        private ProductInfo(string sku, string name, string? description)
        {
            Sku = sku;
            Name = name;
            Description = description;
        }

        public static Result<ProductInfo> Create(string sku, string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return Result<ProductInfo>.Failure(ProductInfoErrors.EmptySku);

            if (string.IsNullOrWhiteSpace(name))
                return Result<ProductInfo>.Failure(ProductInfoErrors.EmptyName);

            return Result<ProductInfo>.Success(new ProductInfo(sku, name, description));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Sku;
            yield return Name;
        }
    }
}
