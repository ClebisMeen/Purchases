using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wex.Purchases.Infrastructure.Treasury.Services;

/// <summary>
/// Retrieves exchange-rate data from the public Treasury API.
/// </summary>
public sealed class TreasuryApiService(HttpClient httpClient, IOptions<TreasuryApiOptions> options) : ITreasuryApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Gets the most recent exchange rate available for the provided currency description and purchase date.
    /// </summary>
    public async Task<GetTreasuryExchangeRateApiResult?> GetExchangeRateAsync(
        GetTreasuryExchangeRateApiRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var apiOptions = options.Value;

        if (string.IsNullOrWhiteSpace(apiOptions.RatesOfExchangeEndpoint))
            throw new InvalidOperationException("Configuration 'TreasuryApi:RatesOfExchangeEndpoint' was not found.");

        var (purchaseDate, minDate) = GetPurchaseAndMinDate(request.PurchaseDate);

        var filters = BuildFilters(request.CountryCurrencyDescription, purchaseDate, minDate);

        var query = BuildQuery(filters);

        var requestUri = $"{apiOptions.RatesOfExchangeEndpoint}?{query}";

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Treasury API request failed with status code {(int)response.StatusCode} ({response.StatusCode}).",
                null,
                response.StatusCode);

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<TreasuryRatesOfExchangeResult>(
            responseStream,
            JsonOptions,
            cancellationToken);

        var item = payload?.Data?.FirstOrDefault();

        if (item is null ||
            string.IsNullOrWhiteSpace(item.Country) ||
            string.IsNullOrWhiteSpace(item.Currency) ||
            string.IsNullOrWhiteSpace(item.CountryCurrencyDescription) ||
            item.ExchangeRate is null ||
            item.RecordDate is null)
        {
            return null;
        }

        var result = new GetTreasuryExchangeRateApiResult(
            item.Country,
            item.Currency,
            item.CountryCurrencyDescription,
            item.ExchangeRate.Value,
            item.RecordDate.Value);

        return result;
    }

    private static string EscapeFilterValue(string value) => value.Replace(",", "\\,", StringComparison.Ordinal);

    private static string BuildFilters(string countryCurrencyDescription, string purchaseDate, string minDate) =>
        string.Join(',',
            $"country_currency_desc:eq:{EscapeFilterValue(countryCurrencyDescription)}", // eq = equals
            $"record_date:lte:{purchaseDate}",  // lte = less than or equal
            $"record_date:gte:{minDate}");      // gte = greater than or equal

    private static string BuildQuery(string filters) =>
        string.Join("&",
            $"filter={Uri.EscapeDataString(filters)}",
            "sort=-record_date",
            "page[size]=1");

    private static (string PurchaseDate, string MinDate) GetPurchaseAndMinDate(DateOnly purchaseDate)
    {
        var minRecordDate = purchaseDate.AddMonths(-6);
        var formattedPurchaseDate = purchaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var formattedMinDate = minRecordDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        return (formattedPurchaseDate, formattedMinDate);
    }

}
