namespace Wex.Purchases.UnitTests.Fixtures;

public sealed class PurchaseFixture
{
    public CreatePurchaseRequest GerarCreatePurchaseRequestValido()
    {
        return new CreatePurchaseRequest(
            Description: "Compra valida",
            TransactionDate: DateTime.UtcNow.Date,
            PurchaseAmount: 10.12m);
    }

    public GetPurchaseConvertedRequest GerarGetPurchaseConvertedRequestValido()
    {
        return new GetPurchaseConvertedRequest(Guid.NewGuid(), "BR");
    }

    public CreatePurchaseResult GerarCreatePurchaseResultValido(CreatePurchaseRequest request)
    {
        return new CreatePurchaseResult(
            Guid.NewGuid(),
            request.Description,
            request.TransactionDate,
            request.PurchaseAmount);
    }

    public GetPurchaseConvertedResult CriarGetPurchaseConvertedResult(
        Guid purchaseId,
        string countryCurrencyDescription)
    {
        return new GetPurchaseConvertedResult(
            purchaseId,
            "Compra valida",
            new DateTime(2026, 05, 09),
            100m,
            "Brazil",
            "Real",
            countryCurrencyDescription,
            5.25m,
            new DateOnly(2026, 05, 08),
            525m);
    }
}
