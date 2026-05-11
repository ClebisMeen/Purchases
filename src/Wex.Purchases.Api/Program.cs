using FluentValidation.AspNetCore;
using Wex.Purchases.Application.DependencyInjection;
using Wex.Purchases.Api.ExceptionHandling;
using Wex.Purchases.Infrastructure.Treasury;
using Wex.Purchases.Infrastructure.MySql.DependencyInjection;
using Wex.Purchases.Infrastructure.MySql.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Formatting.Compact;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Wex.Purchases.Api")
        // Compact JSON keeps container logs structured for Datadog log pipelines.
        .WriteTo.Console(new CompactJsonFormatter()));

    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "Wex.Purchases:";
    });

    builder.Services.AddApplicationServices();
    builder.Services.Decorate<IPurchaseService, PurchaseServiceCachingDecorator>();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddTreasuryInfrastructure(builder.Configuration);

    var app = builder.Build();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PurchaseDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
            diagnosticContext.Set("TraceId", Activity.Current?.TraceId.ToString());
            diagnosticContext.Set("SpanId", Activity.Current?.SpanId.ToString());
        };
    });

    app.Use(async (context, next) =>
    {
        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        using (LogContext.PushProperty("TraceId", Activity.Current?.TraceId.ToString()))
        using (LogContext.PushProperty("SpanId", Activity.Current?.SpanId.ToString()))
        {
            await next();
        }
    });

    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");
    await app.RunAsync();
}
catch (Exception exception) when (exception.GetType().Name == "HostAbortedException")
{
    throw;
}
catch (Exception exception)
{
    Log.Fatal(exception, "Wex Purchases API terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program;
