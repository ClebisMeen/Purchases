namespace Wex.Purchases.Application.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken cancellationToken = default);
    Task<PurchaseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
