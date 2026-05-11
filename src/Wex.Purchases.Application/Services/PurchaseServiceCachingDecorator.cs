using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Wex.Purchases.Application.Currencies;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseServiceCachingDecorator"/> class.
    /// </summary>
    public PurchaseServiceCachingDecorator(
        IPurchaseService inner,
        IDistributedCache cache,
        ILogger<PurchaseServiceCachingDecorator> logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Delegates purchase creation to the inner service without caching.
    /// </summary>
    public Task<CreatePurchaseResult> CreateAsync(
        CreatePurchaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return _inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>
    /// Retrieves a converted purchase from cache when available or from the inner service otherwise.
    /// </summary>
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
