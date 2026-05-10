using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Wex.Purchases.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception while processing request.");

        var statusCode = exception switch
        {
            ArgumentOutOfRangeException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == (int)HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : "Request validation failed.",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
