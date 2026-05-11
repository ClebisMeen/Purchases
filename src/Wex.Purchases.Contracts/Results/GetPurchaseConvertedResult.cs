namespace Wex.Purchases.Contracts.Results;

/// <summary>
/// Represents a purchase enriched with treasury exchange-rate data and converted amount.
/// </summary>
public sealed record GetPurchaseConvertedResult(
    Guid Id,
    string Description,
    DateTime TransactionDate,
    decimal AmountUsd,
    string Country,
    string Currency,
    string CountryCurrencyDescription,
    decimal ExchangeRate,
    DateOnly ExchangeRateRecordDate,
    decimal AmountConverted
)
{
    /// <summary>
    /// Gets the source used to build the result.
    /// </summary>
    public string From { get; init; } = "database";
}
