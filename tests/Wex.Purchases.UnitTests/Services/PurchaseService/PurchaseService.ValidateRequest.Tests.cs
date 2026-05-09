using Wex.Purchases.Application.Services;
using Wex.Purchases.Contracts.Requests;

namespace Wex.Purchases.UnitTests.Services;

public class PurchaseServiceValidateRequestTests
{
    [Fact]
    public void Testar_ValidateRequest_RequestNulo_DeveGerarErro()
    {
        CreatePurchaseRequest request = null!;

        var exception = Assert.Throws<ArgumentNullException>(() => PurchaseService.ValidateRequest(request));

        Assert.Equal("request", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Testar_ValidateRequest_DescriptionInvalida_DeveGerarErro(string description)
    {
        var request = new CreatePurchaseRequest(description, DateTime.UtcNow.Date, 10m);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Description is required. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_DescriptionMaiorQue50_DeveGerarErro()
    {
        var request = new CreatePurchaseRequest(new string('A', 51), DateTime.UtcNow.Date, 10m);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Description must be at most 50 characters. (Parameter 'request')", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Testar_ValidateRequest_PurchaseAmountInvalido_DeveGerarErro(decimal purchaseAmount)
    {
        var request = new CreatePurchaseRequest("Compra valida", DateTime.UtcNow.Date, purchaseAmount);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PurchaseService.ValidateRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseAmount must be positive. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_PurchaseAmountComMaisDeDuasCasasDecimais_DeveGerarErro()
    {
        var request = new CreatePurchaseRequest("Compra valida", DateTime.UtcNow.Date, 10.123m);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseAmount must have at most 2 decimal places. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_TransactionDateDefault_DeveGerarErro()
    {
        var request = new CreatePurchaseRequest("Compra valida", default, 10m);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("TransactionDate must be a valid date. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateRequest_Valido_NaoDeveGerarErro()
    {
        var request = new CreatePurchaseRequest("Compra valida", DateTime.UtcNow.Date, 10.12m);

        var exception = Record.Exception(() => PurchaseService.ValidateRequest(request));

        Assert.Null(exception);
    }
}
