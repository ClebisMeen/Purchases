namespace Wex.Purchases.Infrastructure.Treasury.Options;

/// <summary>
/// Represents configuration settings used to call the Treasury rates API.
/// </summary>
public sealed class TreasuryApiOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "TreasuryApi";

    /// <summary>
    /// Gets the default policy timeout in seconds.
    /// </summary>
    public const int DefaultTimeoutSeconds = 10;

    /// <summary>
    /// Gets the default HTTP client timeout in seconds.
    /// </summary>
    public const int DefaultHttpClientTimeoutSeconds = 100;

    /// <summary>
    /// Gets or sets the Treasury API base URL.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rates-of-exchange endpoint path.
    /// </summary>
    public string RatesOfExchangeEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resilience-policy timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = DefaultTimeoutSeconds;

    /// <summary>
    /// Gets or sets the HTTP client timeout in seconds.
    /// </summary>
    public int HttpClientTimeoutSeconds { get; set; } = DefaultHttpClientTimeoutSeconds;
}
