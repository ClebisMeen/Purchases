using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wex.Purchases.Infrastructure.Treasury.Options;
using Wex.Purchases.Infrastructure.Treasury.Services;

namespace Wex.Purchases.Infrastructure.Treasury;

public static class DependencyInjection
{
    public static IServiceCollection AddTreasuryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TreasuryApiOptions>(configuration.GetSection(TreasuryApiOptions.SectionName));

        services.AddHttpClient<ITreasuryApiService, TreasuryApiService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TreasuryApiOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                throw new InvalidOperationException($"Configuration '{TreasuryApiOptions.SectionName}:BaseUrl' was not found.");
            }

            client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        });

        return services;
    }
}
