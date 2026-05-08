using Microsoft.Extensions.DependencyInjection;
using Wex.Purchases.Application.Services;

namespace Wex.Purchases.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseService, PurchaseService>();
        return services;
    }
}
