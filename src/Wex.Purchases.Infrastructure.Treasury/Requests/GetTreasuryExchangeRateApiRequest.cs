namespace Wex.Purchases.Infrastructure.Treasury.Requests;

public sealed record GetTreasuryExchangeRateApiRequest(
    string CountryCurrencyDescription,
    DateOnly PurchaseDate
);
