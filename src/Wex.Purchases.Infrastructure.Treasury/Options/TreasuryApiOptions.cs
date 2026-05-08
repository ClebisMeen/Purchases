namespace Wex.Purchases.Infrastructure.Treasury.Options;

public sealed class TreasuryApiOptions
{
    public const string SectionName = "TreasuryApi";
    public string BaseUrl { get; set; } = string.Empty;
    public string RatesOfExchangeEndpoint { get; set; } = string.Empty;
}
