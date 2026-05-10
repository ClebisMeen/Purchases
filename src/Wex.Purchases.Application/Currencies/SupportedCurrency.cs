namespace Wex.Purchases.Application.Currencies;

public sealed record SupportedCurrency(
    string CountryCode,
    string CountryName,
    string CurrencyDescription);
