using System.Text.Json.Serialization;

namespace Wex.Purchases.Infrastructure.Treasury.Results;

internal sealed class TreasuryRatesOfExchangeResult
{
    [JsonPropertyName("data")]
    public List<TreasuryRateResult>? Data { get; init; }
}
