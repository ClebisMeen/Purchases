namespace Wex.Purchases.Application.Currencies;

/// <summary>
/// Represents a supported country and its treasury currency description.
/// </summary>
public sealed record SupportedCurrency(
    string CountryCode,
    string CountryName,
    string CurrencyDescription);
