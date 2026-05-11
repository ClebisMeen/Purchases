namespace Wex.Purchases.IntegrationTests;

public sealed class FakeTreasuryApiService : ITreasuryApiService
{
    public Task<GetTreasuryExchangeRateApiResult?> GetExchangeRateAsync(
        GetTreasuryExchangeRateApiRequest request,
        CancellationToken cancellationToken)
    {
        var result = new GetTreasuryExchangeRateApiResult(
            "Brazil",
            "Real",
            request.CountryCurrencyDescription,
            5.00m,
            request.PurchaseDate);

        return Task.FromResult<GetTreasuryExchangeRateApiResult?>(result);
    }
}
