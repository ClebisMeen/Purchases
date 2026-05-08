namespace Wex.Purchases.Contracts.Results;

public sealed record CreatePurchaseResult(
    Guid Id,
    string Description,
    DateTime TransactionDate,
    decimal AmountUsd
);
