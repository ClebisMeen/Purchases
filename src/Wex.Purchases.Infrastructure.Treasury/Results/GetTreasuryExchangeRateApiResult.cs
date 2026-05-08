namespace Wex.Purchases.Infrastructure.Treasury.Results;

public sealed record GetTreasuryExchangeRateApiResult(
    string Country,
    string Currency,
    string CountryCurrencyDescription,
    decimal ExchangeRate,
    DateOnly RecordDate
);
