using Moq;
using Moq.AutoMock;
using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Infrastructure.Treasury.Requests;
using Wex.Purchases.Infrastructure.Treasury.Services;
using Wex.Purchases.UnitTests.Fixtures;

namespace Wex.Purchases.UnitTests.Services;

public class PurchaseServiceCreateAsyncTests : IClassFixture<PurchaseFixture>
{
    private readonly AutoMocker _mocker = new();
    private readonly PurchaseFixture _purchaseFixture;
    private readonly PurchaseService _purchaseService;

    public PurchaseServiceCreateAsyncTests(PurchaseFixture purchaseFixture)
    {
        _purchaseFixture = purchaseFixture;
        _purchaseService = _mocker.CreateInstance<PurchaseService>();
    }

    [Fact]
    public async Task Testar_CreateAsync_Valido()
    {
        var request = _purchaseFixture.GerarCreatePurchaseRequestValido();
        PurchaseTransaction? purchaseSalva = null;

        _mocker.GetMock<IPurchaseRepository>()
            .Setup(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()))
            .Callback<PurchaseTransaction, CancellationToken>((purchase, _) => purchaseSalva = purchase)
            .Returns(Task.CompletedTask);

        var result = await _purchaseService.CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(10.13m, result.AmountUsd);
        Assert.NotNull(purchaseSalva);
        Assert.Equal(result.Id, purchaseSalva!.Id);
        Assert.Equal(10.13m, purchaseSalva.AmountUsd);

        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_CreateAsync_RequestNulo_DeveGerarErro()
    {
        CreatePurchaseRequest request = null!;

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _purchaseService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Testar_CreateAsync_DescriptionInvalida_DeveGerarErro(string description)
    {
        var request = new CreatePurchaseRequest(description, DateTime.UtcNow.Date, 10m);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _purchaseService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Description is required. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_CreateAsync_DescriptionMaiorQue50_DeveGerarErro()
    {
        var request = new CreatePurchaseRequest(new string('A', 51), DateTime.UtcNow.Date, 10m);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _purchaseService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Description must be at most 50 characters. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Testar_CreateAsync_PurchaseAmountInvalido_DeveGerarErro(decimal purchaseAmount)
    {
        var request = new CreatePurchaseRequest("Compra valida", DateTime.UtcNow.Date, purchaseAmount);

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _purchaseService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseAmount must be positive. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_CreateAsync_TransactionDateDefault_DeveGerarErro()
    {
        var request = new CreatePurchaseRequest("Compra valida", default, 10m);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _purchaseService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("TransactionDate must be a valid date. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
