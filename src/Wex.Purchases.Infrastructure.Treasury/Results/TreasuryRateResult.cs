using System.Text.Json.Serialization;

namespace Wex.Purchases.Infrastructure.Treasury.Results;

/// <summary>
/// Represents a raw exchange-rate item returned by the Treasury API.
/// </summary>
internal sealed class TreasuryRateResult
{
    /// <summary>
    /// Gets the country returned by the Treasury API.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    /// <summary>
    /// Gets the currency returned by the Treasury API.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Gets the country currency description returned by the Treasury API.
    /// </summary>
    [JsonPropertyName("country_currency_desc")]
    public string? CountryCurrencyDescription { get; init; }

    /// <summary>
    /// Gets the exchange rate returned by the Treasury API.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Gets the record date returned by the Treasury API.
    /// </summary>
    [JsonPropertyName("record_date")]
    public DateOnly? RecordDate { get; init; }
}
