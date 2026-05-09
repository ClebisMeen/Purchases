using Moq;
using Moq.AutoMock;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Infrastructure.Treasury.Requests;
using Wex.Purchases.Infrastructure.Treasury.Results;
using Wex.Purchases.Infrastructure.Treasury.Services;
using Wex.Purchases.UnitTests.Fixtures;

namespace Wex.Purchases.UnitTests.Services;

public class PurchaseServiceTests : IClassFixture<PurchaseFixture>
{
    private readonly AutoMocker _mocker = new();
    private readonly PurchaseFixture _purchaseFixture;
    private readonly PurchaseService _purchaseService;

    public PurchaseServiceTests(PurchaseFixture purchaseFixture)
    {
        _purchaseFixture = purchaseFixture;
        _purchaseService = _mocker.CreateInstance<PurchaseService>();
    }

    #region PurchaseService public implementation

    [Fact]
    public async Task Testar_Compra_CreatePurchaseAsync_Valido()
    {
        // Arrange
        var request = _purchaseFixture.GerarCreatePurchaseRequestValido();
        PurchaseTransaction? purchaseSalva = null;

        _mocker.GetMock<IPurchaseRepository>()
            .Setup(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()))
            .Callback<PurchaseTransaction, CancellationToken>((purchase, _) => purchaseSalva = purchase)
            .Returns(Task.CompletedTask);

        // Action
        var result = await _purchaseService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(10.13m, result.AmountUsd);
        Assert.NotNull(purchaseSalva);
        Assert.Equal(result.Id, purchaseSalva!.Id);
        Assert.Equal(10.13m, purchaseSalva.AmountUsd);

        _mocker.GetMock<IPurchaseRepository>()
            .Verify(repository => repository.AddAsync(It.IsAny<PurchaseTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region PurchaseService internal implementation

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_RequestNulo_DeveGerarErro()
    {
        // Arrange
        GetPurchaseConvertedRequest request = null!;

        // Action
        var exception = Assert.Throws<ArgumentNullException>(() =>
            PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_PurchaseIdVazio_DeveGerarErro()
    {
        // Arrange
        var request = new GetPurchaseConvertedRequest(Guid.Empty, "BRAZIL-REAL");

        // Action
        var exception = Assert.Throws<ArgumentException>(() =>
            PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseId is required. (Parameter 'request')", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Testar_ValidateGetPurchaseConvertedRequest_CountryCurrencyDescriptionInvalido_DeveGerarErro(string countryCurrencyDescription)
    {
        // Arrange
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCurrencyDescription);

        // Action
        var exception = Assert.Throws<ArgumentException>(() =>
            PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("CountryCurrencyDescription is required. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_Valido_NaoDeveGerarErro()
    {
        // Arrange
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), "BRAZIL-REAL");

        // Action
        var exception = Record.Exception(() =>
            PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Testar_ValidateRequest_RequestNulo_DeveGerarErro()
    {
        // Arrange
        CreatePurchaseRequest request = null!;

        // Action
        var exception = Assert.Throws<ArgumentNullException>(() =>
            PurchaseService.ValidateRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Testar_ValidateRequest_DescriptionInvalida_DeveGerarErro(string description)
    {
        // Arrange
        var request = new CreatePurchaseRequest(
            description,
            DateTime.UtcNow.Date,
            10m);

        // Action
        var exception = Assert.Throws<ArgumentException>(() =>
            PurchaseService.ValidateRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Description is required. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_DescriptionMaiorQue50_DeveGerarErro()
    {
        // Arrange
        var descriptionCom51Caracteres = new string('A', 51);
        var request = new CreatePurchaseRequest(
            descriptionCom51Caracteres,
            DateTime.UtcNow.Date,
            10m);

        // Action
        var exception = Assert.Throws<ArgumentException>(() =>
            PurchaseService.ValidateRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Description must be at most 50 characters. (Parameter 'request')", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Testar_ValidateRequest_AmountUsdInvalido_DeveGerarErro(decimal amountUsd)
    {
        // Arrange
        var request = new CreatePurchaseRequest(
            "Compra valida",
            DateTime.UtcNow.Date,
            amountUsd);

        // Action
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            PurchaseService.ValidateRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("AmountUsd must be positive. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_TransactionDateDefault_DeveGerarErro()
    {
        // Arrange
        var request = new CreatePurchaseRequest(
            "Compra valida",
            default,
            10m);

        // Action
        var exception = Assert.Throws<ArgumentException>(() =>
            PurchaseService.ValidateRequest(request));

        // Assert
        Assert.Equal("request", exception.ParamName);
        Assert.Equal("TransactionDate must be a valid date. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_Valido_NaoDeveGerarErro()
    {
        // Arrange
        var request = new CreatePurchaseRequest(
            "Compra valida",
            DateTime.UtcNow.Date,
            10.12m);

        // Action
        var exception = Record.Exception(() =>
            PurchaseService.ValidateRequest(request));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Testar_GetPurchaseConvertedAsync_SemCambio_DeveGerarErro()
    {
        // Arrange
        var purchaseTransaction = new PurchaseTransaction(
            Guid.NewGuid(),
            "Compra valida",
            new DateTime(2026, 05, 09),
            100m);
        const string countryCurrencyDescription = "BRAZIL-REAL";

        _mocker.GetMock<ITreasuryApiService>()
            .Setup(service => service.GetExchangeRateAsync(
                It.IsAny<GetTreasuryExchangeRateApiRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetTreasuryExchangeRateApiResult?)null);

        // Action
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _purchaseService.GetPurchaseConvertedAsync(
                purchaseTransaction,
                countryCurrencyDescription,
                CancellationToken.None));

        // Assert
        Assert.Equal(
            "No exchange rate was found for 'BRAZIL-REAL' on or before '2026-05-09'.",
            exception.Message);

        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(
                It.IsAny<GetTreasuryExchangeRateApiRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Testar_GetPurchaseConvertedAsync_Valido_DeveRetornarConversao()
    {
        // Arrange
        var purchaseTransaction = new PurchaseTransaction(
            Guid.NewGuid(),
            "Compra valida",
            new DateTime(2026, 05, 09),
            100m);
        const string countryCurrencyDescription = "BRAZIL-REAL";

        var exchangeRateResult = new GetTreasuryExchangeRateApiResult(
            Country: "Brazil",
            Currency: "Real",
            CountryCurrencyDescription: countryCurrencyDescription,
            ExchangeRate: 5.25m,
            RecordDate: new DateOnly(2026, 05, 08));

        _mocker.GetMock<ITreasuryApiService>()
            .Setup(service => service.GetExchangeRateAsync(
                It.Is<GetTreasuryExchangeRateApiRequest>(request =>
                    request.CountryCurrencyDescription == countryCurrencyDescription &&
                    request.PurchaseDate == DateOnly.FromDateTime(purchaseTransaction.TransactionDate)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchangeRateResult);

        // Action
        var result = await _purchaseService.GetPurchaseConvertedAsync(
            purchaseTransaction,
            countryCurrencyDescription,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(purchaseTransaction.Id, result.Id);
        Assert.Equal(purchaseTransaction.Description, result.Description);
        Assert.Equal(purchaseTransaction.TransactionDate, result.TransactionDate);
        Assert.Equal(purchaseTransaction.AmountUsd, result.AmountUsd);
        Assert.Equal("Brazil", result.Country);
        Assert.Equal("Real", result.Currency);
        Assert.Equal(countryCurrencyDescription, result.CountryCurrencyDescription);
        Assert.Equal(5.25m, result.ExchangeRate);
        Assert.Equal(new DateOnly(2026, 05, 08), result.ExchangeRateRecordDate);
        Assert.Equal(525m, result.AmountConverted);

        _mocker.GetMock<ITreasuryApiService>()
            .Verify(service => service.GetExchangeRateAsync(
                It.IsAny<GetTreasuryExchangeRateApiRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
