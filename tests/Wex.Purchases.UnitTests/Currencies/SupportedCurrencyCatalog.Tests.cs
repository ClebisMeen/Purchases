using Wex.Purchases.Application.Currencies;

namespace Wex.Purchases.UnitTests.Currencies;

public class SupportedCurrencyCatalogTests
{
    [Theory]
    [InlineData("BR", "Brazil", "Brazil-Real")]
    [InlineData("CA", "Canada", "Canada-Dollar")]
    [InlineData("MX", "Mexico", "Mexico-Peso")]
    public void Testar_GetByCountryCode_CodigoValido_DeveRetornarMoedaSuportada(
        string countryCode,
        string countryName,
        string currencyDescription)
    {
        var supportedCurrency = SupportedCurrencyCatalog.GetByCountryCode(countryCode);

        Assert.Equal(countryCode, supportedCurrency.CountryCode);
        Assert.Equal(countryName, supportedCurrency.CountryName);
        Assert.Equal(currencyDescription, supportedCurrency.CurrencyDescription);
    }

    [Theory]
    [InlineData("br")]
    [InlineData("Br")]
    [InlineData("bR")]
    public void Testar_GetByCountryCode_CodigoValidoComCaseDiferente_DeveRetornarMoedaSuportada(string countryCode)
    {
        var supportedCurrency = SupportedCurrencyCatalog.GetByCountryCode(countryCode);

        Assert.Equal("BR", supportedCurrency.CountryCode);
        Assert.Equal("Brazil", supportedCurrency.CountryName);
        Assert.Equal("Brazil-Real", supportedCurrency.CurrencyDescription);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("US")]
    [InlineData("Brazil-Real")]
    [InlineData("Canada-Dollar")]
    [InlineData("Mexico-Peso")]
    public void Testar_GetByCountryCode_CodigoInvalido_DeveGerarErro(string countryCode)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            SupportedCurrencyCatalog.GetByCountryCode(countryCode));

        Assert.Equal("countryCode", exception.ParamName);
        Assert.Equal(
            "Invalid countryCode. Accepted values are: BR, CA, MX. (Parameter 'countryCode')",
            exception.Message);
    }

    [Fact]
    public void Testar_All_DeveConterSomenteMoedasSuportadas()
    {
        var currencies = SupportedCurrencyCatalog.All.ToArray();

        Assert.Collection(
            currencies,
            currency => Assert.Equal(new SupportedCurrency("BR", "Brazil", "Brazil-Real"), currency),
            currency => Assert.Equal(new SupportedCurrency("CA", "Canada", "Canada-Dollar"), currency),
            currency => Assert.Equal(new SupportedCurrency("MX", "Mexico", "Mexico-Peso"), currency));
    }
}
