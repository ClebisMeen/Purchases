namespace Wex.Purchases.Infrastructure.MySql.Persistence;

public sealed class PurchaseDbContext : DbContext
{
    public PurchaseDbContext(DbContextOptions<PurchaseDbContext> options) : base(options)
    {
    }

    public DbSet<PurchaseTransaction> PurchaseTransactions => Set<PurchaseTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PurchaseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
