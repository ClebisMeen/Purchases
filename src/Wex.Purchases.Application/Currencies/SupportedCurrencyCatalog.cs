using System.Diagnostics.CodeAnalysis;

namespace Wex.Purchases.Application.Currencies;

/// <summary>
/// Provides the catalog of country codes supported by the conversion flow.
/// </summary>
public static class SupportedCurrencyCatalog
{
    private static readonly SupportedCurrency[] SupportedCurrencies =
    [
        new("BR", "Brazil", "Brazil-Real"),
        new("CA", "Canada", "Canada-Dollar"),
        new("MX", "Mexico", "Mexico-Peso")
    ];

    /// <summary>
    /// Gets all supported currencies.
    /// </summary>
    public static IReadOnlyCollection<SupportedCurrency> All => SupportedCurrencies;

    /// <summary>
    /// Gets the supported country codes formatted for display.
    /// </summary>
    public static string AcceptedCountryCodes =>
        string.Join(", ", SupportedCurrencies.Select(currency => currency.CountryCode));

    /// <summary>
    /// Gets a supported currency by its country code.
    /// </summary>
    public static SupportedCurrency GetByCountryCode(string? countryCode)
    {
        if (TryGetByCountryCode(countryCode, out var supportedCurrency))
        {
            return supportedCurrency;
        }

        throw new ArgumentException(
            $"Invalid countryCode. Accepted values are: {AcceptedCountryCodes}.",
            nameof(countryCode));
    }

    /// <summary>
    /// Determines whether the provided country code is supported.
    /// </summary>
    public static bool IsSupportedCountryCode(string? countryCode)
    {
        return TryGetByCountryCode(countryCode, out _);
    }

    /// <summary>
    /// Attempts to resolve a supported currency by its country code.
    /// </summary>
    public static bool TryGetByCountryCode(
        string? countryCode,
        [NotNullWhen(true)] out SupportedCurrency? supportedCurrency)
    {
        supportedCurrency = null;

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return false;
        }

        supportedCurrency = SupportedCurrencies.FirstOrDefault(currency =>
            string.Equals(currency.CountryCode, countryCode.Trim(), StringComparison.OrdinalIgnoreCase));

        return supportedCurrency is not null;
    }
}
