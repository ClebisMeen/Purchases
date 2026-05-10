using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Infrastructure.MySql.Persistence;
using Wex.Purchases.Infrastructure.Treasury.Options;
using Wex.Purchases.Infrastructure.Treasury.Policies;
using Wex.Purchases.Infrastructure.Treasury.Services;

namespace Wex.Purchases.IntegrationTests.Configurations;

public sealed class ResilientTreasuryFactory(
    FakeTreasuryHttpMessageHandler treasuryHandler,
    Guid existingPurchaseId)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{TreasuryApiOptions.SectionName}:BaseUrl"] = "https://treasury.example.test",
                [$"{TreasuryApiOptions.SectionName}:RatesOfExchangeEndpoint"] = "/rates"
            });
        });

        builder.ConfigureServices(services =>
        {
            var purchaseDbContextDescriptors = services
                .Where(IsPurchaseDbContextRegistration)
                .ToList();

            foreach (var descriptor in purchaseDbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<ITreasuryApiService>();

            var databaseName = Guid.NewGuid().ToString();

            services.AddDbContext<PurchaseDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName);
            });

            services.AddSingleton(treasuryHandler);
            services.AddHttpClient("ResilientTreasury", (serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<TreasuryApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
                client.Timeout = Timeout.InfiniteTimeSpan;
            })
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                serviceProvider.GetRequiredService<FakeTreasuryHttpMessageHandler>())
            .AddPolicyHandler((serviceProvider, _) =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<TreasuryApiService>>();

                return TreasuryHttpPolicies.GetRetryPolicy(logger, _ => TimeSpan.Zero);
            })
            .AddPolicyHandler(_ => TreasuryHttpPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(1)));

            services.AddTransient<ITreasuryApiService>(serviceProvider =>
                new TreasuryApiService(
                    serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("ResilientTreasury"),
                    serviceProvider.GetRequiredService<IOptions<TreasuryApiOptions>>()));

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PurchaseDbContext>();

            db.Database.EnsureCreated();
            db.PurchaseTransactions.Add(new PurchaseTransaction(
                existingPurchaseId,
                "Book purchase",
                new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                100.00m));
            db.SaveChanges();
        });
    }

    private static bool IsPurchaseDbContextRegistration(ServiceDescriptor descriptor)
    {
        if (descriptor.ServiceType == typeof(PurchaseDbContext) ||
            descriptor.ServiceType == typeof(DbContextOptions<PurchaseDbContext>))
        {
            return true;
        }

        return descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericTypeDefinition().FullName ==
                "Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration`1" &&
            descriptor.ServiceType.GenericTypeArguments[0] == typeof(PurchaseDbContext);
    }
}
