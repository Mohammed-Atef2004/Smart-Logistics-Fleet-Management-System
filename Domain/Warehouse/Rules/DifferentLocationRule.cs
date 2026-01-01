using Domain.Common;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Warehouse.Rules
{
    public class DifferentLocationRule : IBusinessRule
    {
        private readonly StorageLocation _current;
        private readonly StorageLocation _newLocation;

        public DifferentLocationRule(StorageLocation current, StorageLocation newLocation)
        {
            _current = current;
            _newLocation = newLocation;
        }

        public bool IsBroken() => _current.Equals(_newLocation);

        public string Message => "New location must be different.";
    }
}

