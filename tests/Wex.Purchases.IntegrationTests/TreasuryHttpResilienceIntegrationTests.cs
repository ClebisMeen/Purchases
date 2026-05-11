namespace Wex.Purchases.IntegrationTests;

public sealed class TreasuryHttpResilienceIntegrationTests : IClassFixture<TreasuryApiFixture>
{
    private static readonly Guid ExistingPurchaseId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private readonly TreasuryApiFixture _treasuryApiFixture;

    public TreasuryHttpResilienceIntegrationTests(TreasuryApiFixture treasuryApiFixture)
    {
        _treasuryApiFixture = treasuryApiFixture;
    }

    [Fact]
    public async Task GetPurchaseById_WhenTreasuryReturns500_RetriesAndReturnsConvertedPurchase()
    {
        // Arrange
        using var treasuryHandler = new FakeTreasuryHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.InternalServerError),
            _ => new HttpResponseMessage(HttpStatusCode.InternalServerError),
            _ => _treasuryApiFixture.CreateSuccessfulTreasuryResponse());

        await using var factory = new ResilientTreasuryFactory(treasuryHandler, ExistingPurchaseId);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/purchases/{ExistingPurchaseId}?countryCode=BR");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, treasuryHandler.RequestCount);

        var result = await response.Content.ReadFromJsonAsync<GetConvertedPurchaseResult>();
        Assert.NotNull(result);
        Assert.Equal(500.00m, result.AmountConverted);
    }

    [Fact]
    public async Task GetPurchaseById_WhenTreasuryReturns400_DoesNotRetry()
    {
        // Arrange
        using var treasuryHandler = new FakeTreasuryHttpMessageHandler(
            _ => new HttpResponseMessage(HttpStatusCode.BadRequest));

        await using var factory = new ResilientTreasuryFactory(treasuryHandler, ExistingPurchaseId);
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/purchases/{ExistingPurchaseId}?countryCode=BR");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(1, treasuryHandler.RequestCount);
    }
}
