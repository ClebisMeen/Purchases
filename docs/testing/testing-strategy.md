# Testing Strategy

The test strategy validates business behavior, infrastructure boundaries, and HTTP application composition without requiring manually installed databases.

## Test Projects

| Project | Purpose |
| --- | --- |
| `tests/Wex.Purchases.UnitTests` | Tests domain, application, validation, caching, supported currency catalog, and Treasury policies. |
| `tests/Wex.Purchases.IntegrationTests` | Tests API behavior through `WebApplicationFactory` and integration-level wiring. |

## Test Tools

| Tool | Purpose |
| --- | --- |
| xUnit | Test framework. |
| Moq | Mocking application and infrastructure dependencies. |
| Moq.AutoMock | Simplifies service construction with mocks. |
| WebApplicationFactory | Hosts the ASP.NET Core API in tests. |
| EF Core InMemory | Replaces MySQL for integration tests. |
| Fake HTTP handlers | Simulate Treasury API responses. |
| Coverlet | Produces coverage data in CI. |

## What Is Tested

| Area | Examples |
| --- | --- |
| Domain | Money rounding and purchase transaction behavior. |
| Validation | Required fields, positive amount, country codes, GUID validation. |
| Application services | Purchase creation, lookup, no-rate behavior, conversion behavior. |
| Cache decorator | Cache hit and cache miss behavior. |
| Treasury policies | Retry and timeout policy behavior. |
| API integration | HTTP endpoints, service registration, in-memory persistence. |

## Why Automated Tests Matter

Automated tests are critical because the API depends on business-sensitive currency rules and an external Treasury data source.

The tests provide confidence that:

- money is rounded consistently
- invalid purchases are rejected
- supported country codes remain explicit
- conversion uses the correct orchestration path
- API endpoints are wired correctly
- CI/CD can block unsafe releases

## Running Tests

```bash
dotnet test tests/Wex.Purchases.UnitTests/Wex.Purchases.UnitTests.csproj
dotnet test tests/Wex.Purchases.IntegrationTests/Wex.Purchases.IntegrationTests.csproj
```

## Related Documentation

- [Unit Tests](unit-tests.md)
- [Integration Tests](integration-tests.md)
- [GitHub Actions](../ci-cd/github-actions.md)
