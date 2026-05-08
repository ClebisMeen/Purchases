using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Domain.Entities;

namespace Wex.Purchases.Infrastructure.Persistence.Repositories;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly PurchaseDbContext _dbContext;

    public PurchaseRepository(PurchaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchaseTransaction);

        await _dbContext.PurchaseTransactions.AddAsync(purchaseTransaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
