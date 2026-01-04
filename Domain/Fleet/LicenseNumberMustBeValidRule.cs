using Domain.Common;

namespace Domain.Fleet
{
    internal class LicenseNumberMustBeValidRule : IBusinessRule
    {
        private readonly string _licenseNumber;
        public LicenseNumberMustBeValidRule(string licenseNumber)
        {
            _licenseNumber = licenseNumber;
        }
        public string Message => "License number must be valid.";

        bool IBusinessRule.IsBroken()
        {
            // Example validation: License number must be alphanumeric and between 5 to 15 characters
            if (string.IsNullOrWhiteSpace(_licenseNumber))
                return true;
            if (_licenseNumber.Length < 5 || _licenseNumber.Length > 15)
                return true;
            foreach (char c in _licenseNumber)
            {
                if (!char.IsLetterOrDigit(c))
                    return true;
            }
            return false;
        }
    }
}