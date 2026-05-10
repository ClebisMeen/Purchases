namespace Wex.Purchases.Contracts.Requests;

/// <summary>
/// Represents a request to create a purchase transaction.
/// </summary>
public sealed record CreatePurchaseRequest(
    string Description,
    DateTime TransactionDate,
    decimal PurchaseAmount
);
