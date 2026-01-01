using Domain.Common;

namespace Domain.Warehouse.Rules
{
    public class InventoryNameRequiredRule : IBusinessRule
    {
        private readonly string _name;

        public InventoryNameRequiredRule(string name)
        {
            _name = name;
        }

        public bool IsBroken() => string.IsNullOrWhiteSpace(_name);

        public string Message => "Name is required.";
    }
}
