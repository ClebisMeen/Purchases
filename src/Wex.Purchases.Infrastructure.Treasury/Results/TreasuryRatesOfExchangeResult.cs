using System.Text.Json.Serialization;

namespace Wex.Purchases.Infrastructure.Treasury.Results;

/// <summary>
/// Represents the Treasury API response envelope for exchange-rate queries.
/// </summary>
internal sealed class TreasuryRatesOfExchangeResult
{
    /// <summary>
    /// Gets the collection of raw exchange-rate items returned by the API.
    /// </summary>
    [JsonPropertyName("data")]
    public List<TreasuryRateResult>? Data { get; init; }
}
