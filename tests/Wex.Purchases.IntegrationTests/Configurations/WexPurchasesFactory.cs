using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Wex.Purchases.Domain.Entities;
using Wex.Purchases.Infrastructure.MySql.Persistence;
using Wex.Purchases.Infrastructure.Treasury.Services;

namespace Wex.Purchases.IntegrationTests;

public sealed class WexPurchasesFactory : WebApplicationFactory<Program>
{
    public static readonly Guid ExistingPurchaseId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ExistingPurchaseId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());

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
            services.AddSingleton<ITreasuryApiService, FakeTreasuryApiService>();

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PurchaseDbContext>();

            db.Database.EnsureCreated();

            db.PurchaseTransactions.AddRange(
                new PurchaseTransaction(
                    ExistingPurchaseId1,
                    "Book purchase",
                    new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                    100.00m),
                new PurchaseTransaction(
                    ExistingPurchaseId2,
                    "Grocery purchase",
                    new DateTime(2026, 5, 2, 0, 0, 0, DateTimeKind.Utc),
                    50.25m));

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
