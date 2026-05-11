# Integration Tests

Integration tests validate API composition and behavior through an in-memory test host.

## WebApplicationFactory

The integration test project uses `WebApplicationFactory<Program>` to boot the API in a controlled test environment.

Benefits:

- exercises real ASP.NET Core routing and middleware
- validates dependency injection
- avoids running a separate web server manually
- supports test-specific service overrides

## In-Memory Database

The integration test configuration replaces MySQL with EF Core InMemory.

This keeps tests:

- deterministic
- fast
- independent from local database installation
- suitable for CI runners

## Treasury Fakes

Treasury integration is replaced by fakes such as:

- `FakeTreasuryApiService`
- `FakeTreasuryHttpMessageHandler`
- `TreasuryApiFixture`

These fakes allow tests to cover successful conversion, no-rate scenarios, and resilience behavior without relying on live Treasury API availability.

## Running Integration Tests

```bash
dotnet test tests/Wex.Purchases.IntegrationTests/Wex.Purchases.IntegrationTests.csproj
```

## Related Documentation

- [Testing Strategy](testing-strategy.md)
- [Treasury Integration](../business/treasury-integration.md)
