using Microsoft.Extensions.Configuration;
using Wex.Purchases.Infrastructure.MySql.Persistence;
using Wex.Purchases.Infrastructure.MySql.Persistence.Repositories;

namespace Wex.Purchases.Infrastructure.MySql.DependencyInjection;

/// <summary>
/// Registers MySQL infrastructure services used by the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MySQL persistence services and repositories.
    /// </summary>
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
