using Domain.Common.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Domain.Warehouse.ValueObjects
{
    [Owned]
    public class StorageLocation : ValueObject
    {
        public string Aisle { get; private set; }
        public string Shelf { get; private set; }
        public string Bin { get; private set; }

        // 👈 EF Core ONLY
        private StorageLocation() { }

        public StorageLocation(string aisle, string shelf, string bin)
        {
            Aisle = aisle;
            Shelf = shelf;
            Bin = bin;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Aisle;
            yield return Shelf;
            yield return Bin;
        }
    }
}
