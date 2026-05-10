using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Services;

namespace Wex.Purchases.UnitTests.Services;

public class PurchaseServiceValidateGetPurchaseConvertedRequestTests
{
    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_RequestNulo_DeveGerarErro()
    {
        GetPurchaseConvertedRequest request = null!;

        var exception = Assert.Throws<ArgumentNullException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_PurchaseIdVazio_DeveGerarErro()
    {
        var request = new GetPurchaseConvertedRequest(Guid.Empty, "BR");

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseId is required. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_PurchaseIdGuidValido_NaoDeveGerarErro()
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), "BR");

        var exception = Record.Exception(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Testar_ValidateGetPurchaseConvertedRequest_CountryCodeObrigatorio_DeveGerarErro(string countryCode)
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCode);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("countryCode is required. (Parameter 'request')", exception.Message);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("Brazil-Real")]
    [InlineData("Canada-Dollar")]
    [InlineData("Mexico-Peso")]
    public void Testar_ValidateGetPurchaseConvertedRequest_CountryCodeInvalido_DeveGerarErro(string countryCode)
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCode);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("Invalid countryCode. Accepted values are: BR, CA, MX. (Parameter 'request')", exception.Message);
    }

    [Theory]
    [InlineData("BR")]
    [InlineData("br")]
    [InlineData("Br")]
    public void Testar_ValidateGetPurchaseConvertedRequest_CountryCodeValido_NaoDeveGerarErro(string countryCode)
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCode);

        var exception = Record.Exception(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Null(exception);
    }
}
