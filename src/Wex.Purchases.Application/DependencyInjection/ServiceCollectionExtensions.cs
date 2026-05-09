using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wex.Purchases.Application.Validators;
using Wex.Purchases.Application.Services;
using Wex.Purchases.Application.Requests;
using Wex.Purchases.Contracts.Requests;

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
