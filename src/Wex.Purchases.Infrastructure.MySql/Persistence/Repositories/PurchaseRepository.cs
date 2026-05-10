namespace Wex.Purchases.Infrastructure.MySql.Persistence.Repositories;

/// <summary>
/// Persists and queries purchase transactions using Entity Framework.
/// </summary>
public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly PurchaseDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseRepository"/> class.
    /// </summary>
    public PurchaseRepository(PurchaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a purchase transaction to the database.
    /// </summary>
    public async Task AddAsync(PurchaseTransaction purchaseTransaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(purchaseTransaction);

        await _dbContext.PurchaseTransactions.AddAsync(purchaseTransaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a purchase transaction by its identifier without tracking.
    /// </summary>
    public async Task<PurchaseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PurchaseTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
