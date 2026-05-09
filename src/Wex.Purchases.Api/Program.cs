using FluentValidation.AspNetCore;
using Wex.Purchases.Application.DependencyInjection;
using Wex.Purchases.Infrastructure.MySql.DependencyInjection;
using Wex.Purchases.Infrastructure.MySql.Persistence;
using Microsoft.EntityFrameworkCore;
using Wex.Purchases.Api.ExceptionHandling;
using Wex.Purchases.Infrastructure.Treasury;
using Wex.Purchases.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
