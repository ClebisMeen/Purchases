using Microsoft.Extensions.Configuration;
using Wex.Purchases.Infrastructure.Treasury.Policies;

namespace Wex.Purchases.Infrastructure.Treasury;

/// <summary>
/// Registers the Treasury API integration and its resilience policies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Treasury infrastructure services to the dependency injection container.
    /// </summary>
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
            client.Timeout = TimeSpan.FromSeconds(GetPositiveValue(
                options.HttpClientTimeoutSeconds,
                TreasuryApiOptions.DefaultHttpClientTimeoutSeconds));
        })
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TreasuryApiService>>();
            return TreasuryHttpPolicies.GetRetryPolicy(logger);
        })
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TreasuryApiOptions>>().Value;
            var timeoutSeconds = GetPositiveValue(options.TimeoutSeconds, TreasuryApiOptions.DefaultTimeoutSeconds);

            return TreasuryHttpPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(timeoutSeconds));
        });

        return services;
    }

    private static int GetPositiveValue(int value, int fallback) => value > 0 ? value : fallback;
}
