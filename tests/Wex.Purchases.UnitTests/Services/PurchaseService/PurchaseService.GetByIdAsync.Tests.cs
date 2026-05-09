using Moq;
using Moq.AutoMock;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Infrastructure.Treasury.Requests;
using Wex.Purchases.Infrastructure.Treasury.Results;
using Wex.Purchases.Infrastructure.Treasury.Services;

namespace Wex.Purchases.UnitTests.Services;

public class PurchaseServiceGetByIdAsyncTests
{
    private readonly AutoMocker _mocker = new();
    private readonly PurchaseService _purchaseService;

    public PurchaseServiceGetByIdAsyncTests()
    {
        _purchaseService = _mocker.CreateInstance<PurchaseService>();
    }

    [Fact]
    public async Task Testar_GetByIdAsync_RequestNulo_DeveGerarErro()
    {
        GetPurchaseConvertedRequest request = null!;

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _purchaseService.GetByIdAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_GetByIdAsync_PurchaseIdVazio_DeveGerarErro()
    {
        var request = new GetPurchaseConvertedRequest(Guid.Empty, "Brazil-Real");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _purchaseService.GetByIdAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseId is required. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Testar_GetByIdAsync_CountryCurrencyDescriptionInvalido_DeveGerarErro(string countryCurrencyDescription)
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCurrencyDescription);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _purchaseService.GetByIdAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("CountryCurrencyDescription is required. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_GetByIdAsync_CountryCurrencyDescriptionForaDoFormatoEsperado_DeveGerarErro()
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), "Brazil");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _purchaseService.GetByIdAsync(request, CancellationToken.None));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("CountryCurrencyDescription must match a supported Treasury API description format, for example: Brazil-Real. (Parameter 'request')", exception.Message);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_GetByIdAsync_CompraNaoEncontrada_DeveRetornarNulo()
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), "Brazil-Real");

        _mocker.GetMock<IPurchaseRepository>()
            .Setup(repository => repository.GetByIdAsync(request.PurchaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseTransaction?)null);

        var result = await _purchaseService.GetByIdAsync(request, CancellationToken.None);

        Assert.Null(result);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.GetByIdAsync(request.PurchaseId, It.IsAny<CancellationToken>()), Times.Once);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Testar_GetByIdAsync_Valido_DeveRetornarCompraConvertida()
    {
        var purchaseId = Guid.NewGuid();
        var request = new GetPurchaseConvertedRequest(purchaseId, "Brazil-Real");
        var purchaseTransaction = new PurchaseTransaction(purchaseId, "Compra valida", new DateTime(2026, 05, 09), 100m);
        var exchangeRateResult = new GetTreasuryExchangeRateApiResult("Brazil", "Real", "Brazil-Real", 5.25m, new DateOnly(2026, 05, 08));

        _mocker.GetMock<IPurchaseRepository>()
            .Setup(repository => repository.GetByIdAsync(purchaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(purchaseTransaction);
        _mocker.GetMock<ITreasuryApiService>()
            .Setup(service => service.GetExchangeRateAsync(
                It.Is<GetTreasuryExchangeRateApiRequest>(treasuryRequest =>
                    treasuryRequest.CountryCurrencyDescription == request.CountryCurrencyDescription &&
                    treasuryRequest.PurchaseDate == DateOnly.FromDateTime(purchaseTransaction.TransactionDate)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchangeRateResult);

        var result = await _purchaseService.GetByIdAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(purchaseId, result!.Id);
        Assert.Equal("Compra valida", result.Description);
        Assert.Equal(100m, result.AmountUsd);
        Assert.Equal("Brazil", result.Country);
        Assert.Equal("Real", result.Currency);
        Assert.Equal("Brazil-Real", result.CountryCurrencyDescription);
        Assert.Equal(5.25m, result.ExchangeRate);
        Assert.Equal(525m, result.AmountConverted);
        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.GetByIdAsync(purchaseId, It.IsAny<CancellationToken>()), Times.Once);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Testar_GetPurchaseConvertedAsync_SemCambio_DeveGerarErro()
    {
        var purchaseTransaction = new PurchaseTransaction(Guid.NewGuid(), "Compra valida", new DateTime(2026, 05, 09), 100m);
        const string countryCurrencyDescription = "Brazil-Real";

        _mocker.GetMock<ITreasuryApiService>()
            .Setup(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetTreasuryExchangeRateApiResult?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _purchaseService.GetPurchaseConvertedAsync(purchaseTransaction, countryCurrencyDescription, CancellationToken.None));

        Assert.Equal("No exchange rate was found for 'BRAZIL-REAL' on or before '2026-05-09'.", exception.Message);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Testar_GetPurchaseConvertedAsync_Valido_DeveRetornarConversao()
    {
        var purchaseTransaction = new PurchaseTransaction(Guid.NewGuid(), "Compra valida", new DateTime(2026, 05, 09), 100m);
        const string countryCurrencyDescription = "Brazil-Real";
        var exchangeRateResult = new GetTreasuryExchangeRateApiResult("Brazil", "Real", countryCurrencyDescription, 5.25m, new DateOnly(2026, 05, 08));

        _mocker.GetMock<ITreasuryApiService>()
            .Setup(service => service.GetExchangeRateAsync(
                It.Is<GetTreasuryExchangeRateApiRequest>(request =>
                    request.CountryCurrencyDescription == countryCurrencyDescription &&
                    request.PurchaseDate == DateOnly.FromDateTime(purchaseTransaction.TransactionDate)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchangeRateResult);

        var result = await _purchaseService.GetPurchaseConvertedAsync(purchaseTransaction, countryCurrencyDescription, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(purchaseTransaction.Id, result.Id);
        Assert.Equal(525m, result.AmountConverted);
        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(It.IsAny<GetTreasuryExchangeRateApiRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
