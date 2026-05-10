using System.Diagnostics.CodeAnalysis;

namespace Wex.Purchases.Application.Currencies;

public static class SupportedCurrencyCatalog
{
    private static readonly SupportedCurrency[] SupportedCurrencies =
    [
        new("BR", "Brazil", "Brazil-Real"),
        new("CA", "Canada", "Canada-Dollar"),
        new("MX", "Mexico", "Mexico-Peso")
    ];

    public static IReadOnlyCollection<SupportedCurrency> All => SupportedCurrencies;

    public static string AcceptedCountryCodes =>
        string.Join(", ", SupportedCurrencies.Select(currency => currency.CountryCode));

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

    public static bool IsSupportedCountryCode(string? countryCode)
    {
        return TryGetByCountryCode(countryCode, out _);
    }

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
