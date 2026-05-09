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
        var request = new GetPurchaseConvertedRequest(Guid.Empty, "Brazil-Real");

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("PurchaseId is required. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_PurchaseIdGuidValido_NaoDeveGerarErro()
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), "Brazil-Real");

        var exception = Record.Exception(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Testar_ValidateGetPurchaseConvertedRequest_CountryCurrencyDescriptionInvalido_DeveGerarErro(string countryCurrencyDescription)
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCurrencyDescription);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("CountryCurrencyDescription is required. (Parameter 'request')", exception.Message);
    }

    [Theory]
    [InlineData("Brazil")]
    [InlineData("Brazil-")]
    [InlineData("-Real")]
    [InlineData("Brazil-Real-Extra")]
    [InlineData("Brazil-123")]
    public void Testar_ValidateGetPurchaseConvertedRequest_CountryCurrencyDescriptionForaDoFormatoEsperado_DeveGerarErro(string countryCurrencyDescription)
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), countryCurrencyDescription);

        var exception = Assert.Throws<ArgumentException>(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Equal("request", exception.ParamName);
        Assert.Equal("CountryCurrencyDescription must match a supported Treasury API description format, for example: Brazil-Real. (Parameter 'request')", exception.Message);
    }

    [Fact]
    public void Testar_ValidateGetPurchaseConvertedRequest_Valido_NaoDeveGerarErro()
    {
        var request = new GetPurchaseConvertedRequest(Guid.NewGuid(), "Brazil-Real");

        var exception = Record.Exception(() => PurchaseService.ValidateGetPurchaseConvertedRequest(request));

        Assert.Null(exception);
    }
}
