using Domain.Common.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.ValueObjects
{
    public class StorageLocation : ValueObject
    {
        public string Aisle { get; }
        public string Shelf { get; }
        public string Bin { get; }

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
