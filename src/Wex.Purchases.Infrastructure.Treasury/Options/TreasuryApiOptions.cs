namespace Wex.Purchases.Infrastructure.Treasury.Options;

public sealed class TreasuryApiOptions
{
    public const string SectionName = "TreasuryApi";
    public const int DefaultTimeoutSeconds = 10;
    public const int DefaultHttpClientTimeoutSeconds = 100;

    public string BaseUrl { get; set; } = string.Empty;
    public string RatesOfExchangeEndpoint { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = DefaultTimeoutSeconds;
    public int HttpClientTimeoutSeconds { get; set; } = DefaultHttpClientTimeoutSeconds;
}
