namespace Wex.Purchases.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static Money FromAmount(decimal amount) => new(amount);

    public Money Convert(decimal exchangeRate) => new(Amount * exchangeRate);
}
