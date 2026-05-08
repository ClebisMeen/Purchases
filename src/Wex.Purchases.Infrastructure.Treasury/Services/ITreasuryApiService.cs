using Wex.Purchases.Infrastructure.Treasury.Requests;
using Wex.Purchases.Infrastructure.Treasury.Results;

namespace Wex.Purchases.Infrastructure.Treasury.Services;

public interface ITreasuryApiService
{
    Task<GetTreasuryExchangeRateApiResult?> GetExchangeRateAsync(
        GetTreasuryExchangeRateApiRequest request,
        CancellationToken cancellationToken);
}
