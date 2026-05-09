namespace Wex.Purchases.Contracts.Requests;

public sealed record CreatePurchaseRequest(
    string Description,
    DateTime TransactionDate,
    decimal PurchaseAmount
);
