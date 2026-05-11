namespace Wex.Purchases.Contracts.Results;

/// <summary>
/// Represents the persisted purchase returned after a successful create operation.
/// </summary>
public sealed record CreatePurchaseResult(
    Guid Id,
    string Description,
    DateTime TransactionDate,
    decimal AmountUsd
);
