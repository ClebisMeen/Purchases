using System.Net;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Wex.Purchases.Infrastructure.Treasury.Policies;

/// <summary>
/// Provides resilience policies used when calling the Treasury API.
/// </summary>
public static class TreasuryHttpPolicies
{
    /// <summary>
    /// Gets the number of retry attempts configured for transient Treasury API failures.
    /// </summary>
    public const int RetryAttempts = 3;

    private static readonly HttpStatusCode[] AdditionalRetryStatusCodes =
    [
        HttpStatusCode.TooManyRequests,
    ];

    /// <summary>
    /// Creates the default retry policy for Treasury API calls.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger) =>
        GetRetryPolicy(logger, GetExponentialBackoffDelay);

    /// <summary>
    /// Creates a retry policy for Treasury API calls using a custom delay strategy.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(
        ILogger logger,
        Func<int, TimeSpan> sleepDurationProvider)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(sleepDurationProvider);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .OrResult(response => AdditionalRetryStatusCodes.Contains(response.StatusCode))
            .WaitAndRetryAsync(
                RetryAttempts,
                sleepDurationProvider,
                (outcome, delay, retryAttempt, _) =>
                {
                    var requestUri = outcome.Result?.RequestMessage?.RequestUri?.ToString();
                    var statusCode = outcome.Result is null ? null : (int?)outcome.Result.StatusCode;
                    var exceptionMessage = outcome.Exception?.Message;

                    logger.LogWarning(
                        outcome.Exception,
                        "Transient failure calling Treasury API. RetryAttempt: {RetryAttempt}; RetryDelaySeconds: {RetryDelaySeconds}; Endpoint: {Endpoint}; StatusCode: {StatusCode}; ExceptionMessage: {ExceptionMessage}",
                        retryAttempt,
                        delay.TotalSeconds,
                        requestUri,
                        statusCode,
                        exceptionMessage);
                });
    }

    /// <summary>
    /// Creates a timeout policy for Treasury API calls.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be greater than zero.");

        return Policy.TimeoutAsync<HttpResponseMessage>(timeout, TimeoutStrategy.Optimistic);
    }

    /// <summary>
    /// Calculates the exponential backoff delay for a retry attempt.
    /// </summary>
    public static TimeSpan GetExponentialBackoffDelay(int retryAttempt) =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
}
