using Wex.Purchases.Application.Validators;
using Wex.Purchases.Application.Services;

namespace Wex.Purchases.Application.DependencyInjection;

/// <summary>
/// Registers application-layer services and validators.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the application services required by the purchases workflow.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreatePurchaseRequest>, CreatePurchaseRequestValidator>();
        services.AddScoped<IValidator<GetPurchaseConvertedRequest>, GetPurchaseConvertedRequestValidator>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        return services;
    }
}
