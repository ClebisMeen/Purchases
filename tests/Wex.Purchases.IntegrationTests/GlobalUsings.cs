// Microsoft
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;

// System
global using System.Net;
global using System.Net.Http.Json;

// Project
global using Wex.Purchases.Contracts.Requests;
global using Wex.Purchases.Contracts.Results;
global using Wex.Purchases.Domain.Entities;
global using Wex.Purchases.Infrastructure.MySql.Persistence;
global using Wex.Purchases.Infrastructure.Treasury.Requests;
global using Wex.Purchases.Infrastructure.Treasury.Services;
global using Wex.Purchases.IntegrationTests.Configurations;
global using Wex.Purchases.IntegrationTests.Fixtures;
