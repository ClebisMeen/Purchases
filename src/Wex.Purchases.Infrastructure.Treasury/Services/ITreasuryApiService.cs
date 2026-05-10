namespace Wex.Purchases.Infrastructure.Treasury.Services;

public interface ITreasuryApiService
{
    Task<GetTreasuryExchangeRateApiResult?> GetExchangeRateAsync(
        GetTreasuryExchangeRateApiRequest request,
        CancellationToken cancellationToken);
}
