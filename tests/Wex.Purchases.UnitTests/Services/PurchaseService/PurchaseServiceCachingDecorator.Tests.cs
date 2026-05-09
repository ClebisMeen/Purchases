using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Moq.AutoMock;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Application.Services;
using Wex.Purchases.UnitTests.Fixtures;

namespace Wex.Purchases.UnitTests.Services;

public class PurchaseServiceCachingDecoratorTests : IClassFixture<PurchaseFixture>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly AutoMocker _mocker = new();
    private readonly PurchaseFixture _purchaseFixture;
    private readonly PurchaseServiceCachingDecorator _decorator;

    public PurchaseServiceCachingDecoratorTests(PurchaseFixture purchaseFixture)
    {
        _purchaseFixture = purchaseFixture;
        _decorator = _mocker.CreateInstance<PurchaseServiceCachingDecorator>();
    }

    [Fact]
    public async Task GetByIdAsync_CacheMiss_DeveBuscarNoInnerESalvarNoCache()
    {
        // Arrange
        var request = _purchaseFixture.GerarGetPurchaseConvertedRequestValido();
        var expectedResult = _purchaseFixture.CriarGetPurchaseConvertedResult(
            request.PurchaseId,
            request.CountryCurrencyDescription);
        var expectedCacheKey = $"purchases:get-by-id:{request.PurchaseId}:{request.CountryCurrencyDescription}";
        byte[]? cachedBytes = null;
        DistributedCacheEntryOptions? cacheOptions = null;

        _mocker.GetMock<IDistributedCache>()
            .Setup(cache => cache.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        _mocker.GetMock<IPurchaseService>()
            .Setup(service => service.GetByIdAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        _mocker.GetMock<IDistributedCache>()
            .Setup(cache => cache.SetAsync(
                expectedCacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((_, value, options, _) =>
            {
                cachedBytes = value;
                cacheOptions = options;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _decorator.GetByIdAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        Assert.Equal("database", result!.From);
        Assert.NotNull(cachedBytes);
        Assert.Equal(expectedResult, JsonSerializer.Deserialize<GetPurchaseConvertedResult>(
            Encoding.UTF8.GetString(cachedBytes!),
            JsonSerializerOptions));
        Assert.NotNull(cacheOptions);
        Assert.Equal(TimeSpan.FromSeconds(10), cacheOptions!.AbsoluteExpirationRelativeToNow);

        _mocker.GetMock<IPurchaseService>()
            .Verify(service => service.GetByIdAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _mocker.GetMock<IDistributedCache>()
            .Verify(cache => cache.SetAsync(
                expectedCacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_CacheHit_DeveRetornarDoCacheENaoChamarInner()
    {
        // Arrange
        var request = _purchaseFixture.GerarGetPurchaseConvertedRequestValido();
        var expectedResult = _purchaseFixture.CriarGetPurchaseConvertedResult(
            request.PurchaseId,
            request.CountryCurrencyDescription);
        var expectedCacheKey = $"purchases:get-by-id:{request.PurchaseId}:{request.CountryCurrencyDescription}";
        var cachedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expectedResult, JsonSerializerOptions));

        _mocker.GetMock<IDistributedCache>()
            .Setup(cache => cache.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedBytes);

        // Act
        var result = await _decorator.GetByIdAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult with { From = "cache" }, result);
        Assert.Equal("cache", result!.From);
        _mocker.GetMock<IPurchaseService>()
            .Verify(service => service.GetByIdAsync(It.IsAny<GetPurchaseConvertedRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<IDistributedCache>()
            .Verify(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_RequestInvalido_DeveGerarErroSemAcessarCacheOuInner()
    {
        // Arrange
        var request = new GetPurchaseConvertedRequest(Guid.Empty, "Brazil-Real");

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _decorator.GetByIdAsync(request, CancellationToken.None));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseId is required. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseService>()
            .Verify(service => service.GetByIdAsync(It.IsAny<GetPurchaseConvertedRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<IDistributedCache>()
            .Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<IDistributedCache>()
            .Verify(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DeveDelegarParaInnerSemUsarCache()
    {
        // Arrange
        var request = _purchaseFixture.GerarCreatePurchaseRequestValido();
        var expectedResult = _purchaseFixture.GerarCreatePurchaseResultValido(request);

        _mocker.GetMock<IPurchaseService>()
            .Setup(service => service.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _decorator.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        _mocker.GetMock<IPurchaseService>()
            .Verify(service => service.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _mocker.GetMock<IDistributedCache>()
            .Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<IDistributedCache>()
            .Verify(cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);
    }
}
