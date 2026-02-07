using Domain.Common;

public class PositiveAmountRule : IBusinessRule
{
    private readonly int _amount;

    public PositiveAmountRule(int amount)
    {
        _amount = amount;
    }

    public bool IsBroken() => _amount <= 0;

    public string Message => "Amount must be positive.";
}
