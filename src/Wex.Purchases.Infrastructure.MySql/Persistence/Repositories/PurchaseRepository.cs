namespace Wex.Purchases.Infrastructure.MySql.Persistence.Repositories;

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

    public async Task<PurchaseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PurchaseTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
