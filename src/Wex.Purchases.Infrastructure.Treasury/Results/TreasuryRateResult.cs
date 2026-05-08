using System.Text.Json.Serialization;

namespace Wex.Purchases.Infrastructure.Treasury.Results;

internal sealed class TreasuryRateResult
{
    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("country_currency_desc")]
    public string? CountryCurrencyDescription { get; init; }

    [JsonPropertyName("exchange_rate")]
    public decimal? ExchangeRate { get; init; }

    [JsonPropertyName("record_date")]
    public DateOnly? RecordDate { get; init; }
}
