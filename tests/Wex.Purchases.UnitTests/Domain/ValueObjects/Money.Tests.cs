using Wex.Purchases.Domain.ValueObjects;

namespace Wex.Purchases.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(10, 10)]
    [InlineData(10.124, 10.12)]
    [InlineData(10.125, 10.13)]
    [InlineData(10.126, 10.13)]
    [InlineData(-10.124, -10.12)]
    [InlineData(-10.125, -10.13)]
    [InlineData(-10.126, -10.13)]
    [InlineData(0, 0)]
    [InlineData(123.456789, 123.46)]
    public void Testar_FromAmount_DeveArredondarComDuasCasasDecimais(decimal amount, decimal expectedAmount)
    {
        var money = Money.FromAmount(amount);

        Assert.Equal(expectedAmount, money.Amount);
    }

    [Fact]
    public void Testar_Convert_Valido_DeveMultiplicarEArredondar()
    {
        var money = Money.FromAmount(10.13m);

        var converted = money.Convert(5.67m);

        Assert.Equal(10.13m, money.Amount);
        Assert.Equal(57.44m, converted.Amount);
    }

    [Theory]
    [InlineData(10.125, 1, 10.13)]
    [InlineData(10.125, 1.5, 15.20)]
    [InlineData(10.125, 0.5, 5.07)]
    [InlineData(10.12, 0, 0)]
    [InlineData(10.12, -1.5, -15.18)]
    [InlineData(-10.125, 1, -10.13)]
    [InlineData(-10.125, 0.5, -5.07)]
    [InlineData(1, 1.005, 1.01)]
    [InlineData(-1, 1.005, -1.01)]
    public void Testar_Convert_DeveAplicarArredondamentoAwayFromZero(decimal amount, decimal exchangeRate, decimal expectedAmount)
    {
        var money = Money.FromAmount(amount);

        var converted = money.Convert(exchangeRate);

        Assert.Equal(expectedAmount, converted.Amount);
    }

    [Fact]
    public void Testar_Convert_NaoDeveAlterarObjetoOriginal()
    {
        var money = Money.FromAmount(10.125m);

        var converted = money.Convert(2m);

        Assert.Equal(10.13m, money.Amount);
        Assert.Equal(20.26m, converted.Amount);
    }

    [Fact]
    public void Testar_RecordEquality_DeveConsiderarAmountArredondado()
    {
        var moneyA = Money.FromAmount(10.125m);
        var moneyB = Money.FromAmount(10.13m);

        Assert.Equal(moneyA, moneyB);
        Assert.True(moneyA == moneyB);
    }
}
