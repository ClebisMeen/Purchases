using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wex.Purchases.Infrastructure.MySql.Persistence;

public sealed class PurchaseDbContextFactory : IDesignTimeDbContextFactory<PurchaseDbContext>
{
    public PurchaseDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PurchaseDb")
            ?? throw new InvalidOperationException("Connection string 'PurchaseDb' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<PurchaseDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 4, 0)));

        return new PurchaseDbContext(optionsBuilder.Options);
    }
}
