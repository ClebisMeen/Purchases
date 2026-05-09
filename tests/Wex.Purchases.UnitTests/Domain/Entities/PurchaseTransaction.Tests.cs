using Wex.Purchases.Domain.Entities;

namespace Wex.Purchases.UnitTests.Domain.Entities;

public class PurchaseTransactionTests
{
    [Fact]
    public void Testar_Constructor_Valido()
    {
        var id = Guid.NewGuid();
        var transactionDate = new DateTime(2026, 5, 9);

        var purchaseTransaction = new PurchaseTransaction(id, "Compra valida", transactionDate, 10.13m);

        Assert.Equal(id, purchaseTransaction.Id);
        Assert.Equal("Compra valida", purchaseTransaction.Description);
        Assert.Equal(transactionDate, purchaseTransaction.TransactionDate);
        Assert.Equal(10.13m, purchaseTransaction.AmountUsd);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Testar_Constructor_DescriptionInvalida_DeveGerarErro(string description)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PurchaseTransaction(Guid.NewGuid(), description, DateTime.UtcNow.Date, 10m));

        Assert.Equal("description", exception.ParamName);
        Assert.Equal("Description is required. (Parameter 'description')", exception.Message);
    }

    [Fact]
    public void Testar_Constructor_DescriptionMaiorQue50_DeveGerarErro()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PurchaseTransaction(Guid.NewGuid(), new string('A', 51), DateTime.UtcNow.Date, 10m));

        Assert.Equal("description", exception.ParamName);
        Assert.Equal("Description must be at most 50 characters. (Parameter 'description')", exception.Message);
    }

    [Fact]
    public void Testar_Constructor_TransactionDateDefault_DeveGerarErro()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PurchaseTransaction(Guid.NewGuid(), "Compra valida", default, 10m));

        Assert.Equal("transactionDate", exception.ParamName);
        Assert.Equal("TransactionDate must be a valid date. (Parameter 'transactionDate')", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Testar_Constructor_AmountUsdInvalido_DeveGerarErro(decimal amountUsd)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PurchaseTransaction(Guid.NewGuid(), "Compra valida", DateTime.UtcNow.Date, amountUsd));

        Assert.Equal("amountUsd", exception.ParamName);
        Assert.Equal("AmountUsd must be positive. (Parameter 'amountUsd')", exception.Message);
    }

    [Theory]
    [InlineData(10.124, 10.12)]
    [InlineData(10.125, 10.13)]
    [InlineData(10.126, 10.13)]
    public void Testar_Constructor_AmountUsd_DeveArredondarComDuasCasasDecimais(decimal amountUsd, decimal expectedAmountUsd)
    {
        var purchaseTransaction = new PurchaseTransaction(Guid.NewGuid(), "Compra valida", DateTime.UtcNow.Date, amountUsd);

        Assert.Equal(expectedAmountUsd, purchaseTransaction.AmountUsd);
    }
}
