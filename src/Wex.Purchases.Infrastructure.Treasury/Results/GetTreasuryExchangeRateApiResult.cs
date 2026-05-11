namespace Wex.Purchases.Infrastructure.Treasury.Results;

/// <summary>
/// Represents the normalized exchange-rate data returned by the Treasury integration.
/// </summary>
public sealed record GetTreasuryExchangeRateApiResult(
    string Country,
    string Currency,
    string CountryCurrencyDescription,
    decimal ExchangeRate,
    DateOnly RecordDate
);
