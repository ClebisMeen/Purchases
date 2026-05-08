using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wex.Purchases.Application.Repositories;
using Wex.Purchases.Infrastructure.Persistence;
using Wex.Purchases.Infrastructure.Persistence.Repositories;

namespace Wex.Purchases.Infrastructure.DependencyInjection;

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
