using Moq;
using Moq.AutoMock;
using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Domain.Entities;
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
}
