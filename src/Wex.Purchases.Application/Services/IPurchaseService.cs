namespace Wex.Purchases.Application.Services;

/// <summary>
/// Defines the operations for creating purchases and obtaining converted purchase data.
/// </summary>
public interface IPurchaseService
{
    /// <summary>
    /// Creates a new purchase transaction.
    /// </summary>
    Task<CreatePurchaseResult> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a purchase and converts its amount using treasury exchange-rate data.
    /// </summary>
    Task<GetPurchaseConvertedResult?> GetByIdAsync(
        GetPurchaseConvertedRequest request,
        CancellationToken cancellationToken = default);
}
