namespace Wex.Purchases.Contracts.Results;

/// <summary>
/// Represents a purchase result containing the converted amount and exchange rate used.
/// </summary>
public sealed record GetConvertedPurchaseResult(
    Guid Id,
    string Description,
    DateTime TransactionDate,
    decimal AmountUsd,
    decimal ConversionRate,
    decimal AmountConverted
);
