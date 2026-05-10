namespace Wex.Purchases.Infrastructure.Treasury.Services;

/// <summary>
/// Defines operations for querying exchange rates from the Treasury API.
/// </summary>
public interface ITreasuryApiService
{
    /// <summary>
    /// Gets the most recent exchange rate available for the requested currency description and purchase date.
    /// </summary>
    Task<GetTreasuryExchangeRateApiResult?> GetExchangeRateAsync(
        GetTreasuryExchangeRateApiRequest request,
        CancellationToken cancellationToken);
}
