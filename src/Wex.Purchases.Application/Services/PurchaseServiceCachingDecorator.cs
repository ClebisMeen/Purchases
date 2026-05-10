using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Wex.Purchases.Application.Currencies;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;

namespace Wex.Purchases.Application.Services;
/// <summary>
/// A caching decorator for the <see cref="IPurchaseService"/> that caches the results of the GetByIdAsync method using an <see cref="IDistributedCache"/>.
/// Referência: https://github.com/brunobritodev/DecoratorDemo
/// </summary>
public sealed class PurchaseServiceCachingDecorator : IPurchaseService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IPurchaseService _inner;
    private readonly IDistributedCache _cache;
    private readonly ILogger<PurchaseServiceCachingDecorator> _logger;

    public PurchaseServiceCachingDecorator(
        IPurchaseService inner,
        IDistributedCache cache,
        ILogger<PurchaseServiceCachingDecorator> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CreatePurchaseResult> CreateAsync(
        CreatePurchaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return _inner.CreateAsync(request, cancellationToken);
    }

    public async Task<GetPurchaseConvertedResult?> GetByIdAsync(
        GetPurchaseConvertedRequest request,
        CancellationToken cancellationToken = default)
    {
        PurchaseService.ValidateGetPurchaseConvertedRequest(request);

        var supportedCurrency = SupportedCurrencyCatalog.GetByCountryCode(request.CountryCode);
        var cacheKey = CreateGetByIdCacheKey(request.PurchaseId, supportedCurrency.CountryCode);
        var cachedResult = await _cache.GetStringAsync(cacheKey, cancellationToken);


        GetPurchaseConvertedResult? result = null;
        if (cachedResult is not null)
        {
            _logger.LogInformation("Cache hit for purchase GET by ID. CacheKey: {CacheKey}", cacheKey);

            result = JsonSerializer.Deserialize<GetPurchaseConvertedResult?>(
                cachedResult,
                JsonSerializerOptions);
            
            return result is null ? null : result with { From = "cache" };
        }

        _logger.LogInformation("Cache miss for purchase GET by ID. CacheKey: {CacheKey}", cacheKey);

        result = await _inner.GetByIdAsync(request, cancellationToken);
        var serializedResult = JsonSerializer.Serialize(result, JsonSerializerOptions);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        };

        await _cache.SetStringAsync(cacheKey, serializedResult, cacheOptions, cancellationToken);

        _logger.LogInformation("Cache set for purchase GET by ID. CacheKey: {CacheKey}", cacheKey);

        return result;
    }

    private static string CreateGetByIdCacheKey(Guid purchaseId, string countryCode)
    {
        return $"purchases:get-by-id:{purchaseId}:{countryCode}";
    }
}
