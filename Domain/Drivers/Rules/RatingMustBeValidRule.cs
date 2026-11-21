using Domain.Drivers.Errors;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

/// <summary>
/// A trip rating must be a real number in the 1..5 range. The previous
/// inline check (<c>rating is &lt; 1 or &gt; 5</c>) silently accepted
/// <see cref="double.NaN"/> and poisoned the running average.
/// </summary>
public sealed class RatingMustBeValidRule : IBusinessRule
{
    private readonly double _rating;

    public RatingMustBeValidRule(double rating) => _rating = rating;

    public bool IsBroken() =>
        double.IsNaN(_rating) || double.IsInfinity(_rating) || _rating < 1 || _rating > 5;

    public Error Error => DriverErrors.InvalidRating;
}
