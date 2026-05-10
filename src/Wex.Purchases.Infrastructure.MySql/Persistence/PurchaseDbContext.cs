namespace Wex.Purchases.Infrastructure.MySql.Persistence;

/// <summary>
/// Entity Framework database context for purchase persistence.
/// </summary>
public sealed class PurchaseDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PurchaseDbContext"/> class.
    /// </summary>
    public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets the purchase transactions set.
    /// </summary>
    public DbSet<PurchaseTransaction> PurchaseTransactions => Set<PurchaseTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PurchaseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
