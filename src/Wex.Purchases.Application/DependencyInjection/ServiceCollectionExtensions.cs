using Wex.Purchases.Application.Validators;
using Wex.Purchases.Application.Services;

namespace Wex.Purchases.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreatePurchaseRequest>, CreatePurchaseRequestValidator>();
        services.AddScoped<IValidator<GetPurchaseConvertedRequest>, GetPurchaseConvertedRequestValidator>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        return services;
    }
}
