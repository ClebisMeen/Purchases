using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wex.Purchases.Infrastructure.Persistence;

public sealed class PurchaseDbContextFactory : IDesignTimeDbContextFactory<PurchaseDbContext>
{
    public PurchaseDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PurchaseDb")
            ?? "server=localhost;port=3306;database=wex_purchases;user=wex;password=wex123";

        var optionsBuilder = new DbContextOptionsBuilder<PurchaseDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new PurchaseDbContext(optionsBuilder.Options);
    }
}
