using Microsoft.Extensions.Configuration;
using Wex.Purchases.Infrastructure.MySql.Persistence;
using Wex.Purchases.Infrastructure.MySql.Persistence.Repositories;

namespace Wex.Purchases.Infrastructure.MySql.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PurchaseDb")
            ?? throw new InvalidOperationException("Connection string 'PurchaseDb' was not found.");

        services.AddDbContext<PurchaseDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        return services;
    }
}
