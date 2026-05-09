using Wex.Purchases.Contracts.Requests;

namespace Wex.Purchases.UnitTests.Fixtures;

public sealed class PurchaseFixture
{
    public CreatePurchaseRequest GerarCreatePurchaseRequestValido()
    {
        return new CreatePurchaseRequest(
            Description: "Compra valida",
            TransactionDate: DateTime.UtcNow.Date,
            AmountUsd: 10.125m);
    }
}
