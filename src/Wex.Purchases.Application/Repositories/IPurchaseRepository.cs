namespace Wex.Purchases.Application.Repositories;

/// <summary>
/// Defines persistence operations for purchase transactions.
/// </summary>
public interface IPurchaseRepository
{
    /// <summary>
    /// Persists a purchase transaction.
    /// </summary>
    Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a purchase transaction by its identifier.
    /// </summary>
    Task<PurchaseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
