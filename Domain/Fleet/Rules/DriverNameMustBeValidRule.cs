using Domain.Common;

namespace Domain.Fleet.Rules
{
    public class DriverNameMustBeValidRule : IBusinessRule
    {
        private readonly string _fullName;
        public DriverNameMustBeValidRule(string fullName)
        {
            _fullName = fullName;
        }
        string IBusinessRule.Message => "Driver name must be at least 3 characters long and contain only letters and spaces.";

        bool IBusinessRule.IsBroken()
        {
            return _fullName.Length < 3 || !_fullName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }
    }
}