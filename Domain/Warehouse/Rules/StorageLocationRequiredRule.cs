using Domain.Common;
using Domain.Warehouse.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Rules
{
    public class StorageLocationRequiredRule : IBusinessRule
    {
        private readonly StorageLocation _location;

        public StorageLocationRequiredRule(StorageLocation location)
        {
            _location = location;
        }

        public bool IsBroken() => _location == null;

        public string Message => "Location is required.";
    }
}

