namespace Wex.Purchases.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount rounded to two decimal places.
/// </summary>
public sealed record Money : ValueObject
{
    /// <summary>
    /// Gets the normalized monetary amount.
    /// </summary>
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Creates a money value from a raw decimal amount.
    /// </summary>
    public static Money FromAmount(decimal amount) => new(amount);

    /// <summary>
    /// Converts the amount using the provided exchange rate.
    /// </summary>
    public Money Convert(decimal exchangeRate)
    {
        if (exchangeRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exchangeRate), "ExchangeRate must be positive.");
        }

        return new Money(Amount * exchangeRate);
    }
}
