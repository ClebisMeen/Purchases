using Wex.Purchases.Application.Requests;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;

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

    public GetPurchaseConvertedRequest GerarGetPurchaseConvertedRequestValido()
    {
        return new GetPurchaseConvertedRequest(Guid.NewGuid(), "BRAZIL-REAL");
    }

    public CreatePurchaseResult GerarCreatePurchaseResultValido(CreatePurchaseRequest request)
    {
        return new CreatePurchaseResult(
            Guid.NewGuid(),
            request.Description,
            request.TransactionDate,
            request.AmountUsd);
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
