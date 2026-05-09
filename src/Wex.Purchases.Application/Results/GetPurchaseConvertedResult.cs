namespace Wex.Purchases.Application.Results;

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
);
