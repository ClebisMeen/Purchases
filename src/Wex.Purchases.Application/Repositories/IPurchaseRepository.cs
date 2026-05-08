using Wex.Purchases.Domain.Entities;

namespace Wex.Purchases.Application.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken cancellationToken = default);
}
