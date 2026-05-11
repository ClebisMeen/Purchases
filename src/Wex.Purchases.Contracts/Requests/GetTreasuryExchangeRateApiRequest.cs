namespace Wex.Purchases.Contracts.Requests;

/// <summary>
/// Represents the input required to query the Treasury exchange-rate API.
/// </summary>
public sealed record GetTreasuryExchangeRateApiRequest(
    string CountryCurrencyDescription,
    DateOnly PurchaseDate
);
