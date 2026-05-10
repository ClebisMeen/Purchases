using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Timeout;
using Wex.Purchases.Infrastructure.Treasury.Policies;

namespace Wex.Purchases.UnitTests.Infrastructure.Treasury;

public sealed class TreasuryHttpPoliciesTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 8)]
    public void GetExponentialBackoffDelay_WhenRetryAttemptIsProvided_ReturnsExpectedDelay(
        int retryAttempt,
        int expectedSeconds)
    {
        // Act
        var delay = TreasuryHttpPolicies.GetExponentialBackoffDelay(retryAttempt);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), delay);
    }

    [Fact]
    public async Task RetryPolicy_WhenResponseIsInternalServerError_RetriesExpectedAttempts()
    {
        // Arrange
        using var handler = new SequenceHttpMessageHandler(
            HttpStatusCode.InternalServerError,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.OK);

        using var client = new HttpClient(handler);
        var policy = CreateRetryPolicy();

        // Act
        using var response = await policy.ExecuteAsync(
            cancellationToken => client.GetAsync("https://treasury.example.test/rates", cancellationToken),
            CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, handler.RequestCount);
    }

    [Fact]
    public async Task RetryPolicy_WhenResponseIsBadRequest_DoesNotRetry()
    {
        // Arrange
        using var handler = new SequenceHttpMessageHandler(HttpStatusCode.BadRequest);
        using var client = new HttpClient(handler);
        var policy = CreateRetryPolicy();

        // Act
        using var response = await policy.ExecuteAsync(
            cancellationToken => client.GetAsync("https://treasury.example.test/rates", cancellationToken),
            CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task RetryPolicy_WhenResponseRemainsTransient_RespectsMaximumAttempts()
    {
        // Arrange
        using var handler = new SequenceHttpMessageHandler(
            HttpStatusCode.InternalServerError,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.InternalServerError);

        using var client = new HttpClient(handler);
        var policy = CreateRetryPolicy();

        // Act
        using var response = await policy.ExecuteAsync(
            cancellationToken => client.GetAsync("https://treasury.example.test/rates", cancellationToken),
            CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(TreasuryHttpPolicies.RetryAttempts + 1, handler.RequestCount);
    }

    [Fact]
    public async Task RetryPolicy_WhenTimeoutOccurs_RetriesExpectedAttempts()
    {
        // Arrange
        using var handler = new TimeoutHttpMessageHandler();
        using var client = new HttpClient(handler);
        var retryPolicy = CreateRetryPolicy();
        var timeoutPolicy = TreasuryHttpPolicies.GetTimeoutPolicy(TimeSpan.FromMilliseconds(10));
        var policy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

        // Act
        await Assert.ThrowsAsync<TimeoutRejectedException>(() =>
            policy.ExecuteAsync(
                cancellationToken => client.GetAsync("https://treasury.example.test/rates", cancellationToken),
                CancellationToken.None));

        // Assert
        Assert.Equal(TreasuryHttpPolicies.RetryAttempts + 1, handler.RequestCount);
    }

    [Fact]
    public async Task RetryPolicy_WhenCancellationTokenIsCanceled_DoesNotRetry()
    {
        // Arrange
        using var handler = new SequenceHttpMessageHandler(HttpStatusCode.OK);
        using var client = new HttpClient(handler);
        using var cancellationTokenSource = new CancellationTokenSource();
        var policy = CreateRetryPolicy();

        await cancellationTokenSource.CancelAsync();

        // Act
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            policy.ExecuteAsync(
                cancellationToken => client.GetAsync("https://treasury.example.test/rates", cancellationToken),
                cancellationTokenSource.Token));

        // Assert
        Assert.Equal(0, handler.RequestCount);
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        var logger = new Mock<ILogger>();

        return TreasuryHttpPolicies.GetRetryPolicy(
            logger.Object,
            _ => TimeSpan.Zero);
    }

    private sealed class SequenceHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpStatusCode> _statusCodes;

        public SequenceHttpMessageHandler(params HttpStatusCode[] statusCodes)
        {
            _statusCodes = new Queue<HttpStatusCode>(statusCodes);
        }

        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            RequestCount++;
            var statusCode = _statusCodes.Count == 0
                ? HttpStatusCode.OK
                : _statusCodes.Dequeue();

            var response = new HttpResponseMessage(statusCode)
            {
                RequestMessage = request
            };

            return Task.FromResult(response);
        }
    }

    private sealed class TimeoutHttpMessageHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request
            };
        }
    }
}
