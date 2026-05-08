namespace Wex.Purchases.Contracts.Results;

public sealed record GetConvertedPurchaseResult(
    Guid Id,
    string Description,
    DateTime TransactionDate,
    decimal AmountUsd,
    decimal ConversionRate,
    decimal AmountConverted
);
