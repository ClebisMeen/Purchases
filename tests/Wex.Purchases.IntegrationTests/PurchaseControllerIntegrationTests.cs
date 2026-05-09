using System.Net;
using System.Net.Http.Json;
using Wex.Purchases.Application.Results;
using Wex.Purchases.Contracts.Requests;
using Wex.Purchases.Contracts.Results;

namespace Wex.Purchases.IntegrationTests;

public sealed class PurchaseControllerIntegrationTests : IClassFixture<WexPurchasesFactory>
{
    private readonly HttpClient _client;

    public PurchaseControllerIntegrationTests(WexPurchasesFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPurchaseById_WhenPurchaseExists_ReturnsConvertedPurchase()
    {
        // Arrange
        var purchaseId = WexPurchasesFactory.ExistingPurchaseId1;
        var currencyDescription = "Brazil-Real";
        var url = $"/api/purchases/{purchaseId}?countryCurrencyDescription={currencyDescription}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GetPurchaseConvertedResult>();
        Assert.NotNull(result);
        Assert.Equal(purchaseId, result.Id);
        Assert.Equal("Book purchase", result.Description);
        Assert.Equal(currencyDescription, result.CountryCurrencyDescription);
        Assert.Equal(500.00m, result.AmountConverted);
    }

    [Fact]
    public async Task CreatePurchase_WhenRequestIsValid_ReturnsCreatedPurchase()
    {
        // Arrange
        var request = new CreatePurchaseRequest(
            "Headphones",
            new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
            123.45m);

        // Act
        var response = await _client.PostAsJsonAsync("/api/purchases", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CreatePurchaseResult>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.TransactionDate, result.TransactionDate);
        Assert.Equal(request.AmountUsd, result.AmountUsd);
    }
}
