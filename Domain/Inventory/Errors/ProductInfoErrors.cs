using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.Errors
{
    public class ProductInfoErrors
    {
        public static Error EmptySku = new("ProductInfo.EmptySku", "SKU cannot be empty.");
        public static Error EmptyName = new("ProductInfo.EmptyName", "Product name cannot be empty.");
        public static Error DuplicateSku = new("ProductInfo.DuplicateSku", "Sku Is Dublicated");
    }
}
