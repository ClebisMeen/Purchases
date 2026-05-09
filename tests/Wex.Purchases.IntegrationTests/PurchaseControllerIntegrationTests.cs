using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
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
    public async Task GetPurchaseById_WhenCountryCurrencyDescriptionIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var purchaseId = WexPurchasesFactory.ExistingPurchaseId1;
        var url = $"/api/purchases/{purchaseId}?countryCurrencyDescription=";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("Request validation failed.", problemDetails.Title);
    }

    [Fact]
    public async Task GetPurchaseById_WhenIdentifierIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var url = "/api/purchases/not-a-guid?countryCurrencyDescription=Brazil-Real";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
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

    [Fact]
    public async Task CreatePurchase_WhenRequestIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePurchaseRequest(
            "",
            default,
            123.456m);

        // Act
        var response = await _client.PostAsJsonAsync("/api/purchases", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("Description", problemDetails.Errors.Keys);
        Assert.Contains("TransactionDate", problemDetails.Errors.Keys);
        Assert.Contains("AmountUsd", problemDetails.Errors.Keys);
    }
}
