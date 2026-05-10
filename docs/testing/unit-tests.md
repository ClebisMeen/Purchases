# Unit Tests

Unit tests validate isolated domain and application behavior.

## Coverage Areas

| Area | Example Test Files |
| --- | --- |
| Domain value objects | `Money.Tests.cs` |
| Domain entities | `PurchaseTransaction.Tests.cs` |
| Supported currencies | `SupportedCurrencyCatalog.Tests.cs` |
| Purchase service creation | `PurchaseService.CreateAsync.Tests.cs` |
| Purchase service retrieval | `PurchaseService.GetByIdAsync.Tests.cs` |
| Request validation | `PurchaseService.ValidateRequest.Tests.cs` |
| Converted request validation | `PurchaseService.ValidateGetPurchaseConvertedRequest.Tests.cs` |
| Cache decorator | `PurchaseServiceCachingDecorator.Tests.cs` |
| Treasury resilience policies | `TreasuryHttpPoliciesTests.cs` |

## Mocking

Unit tests use Moq and Moq.AutoMock to replace dependencies such as:

- `IPurchaseRepository`
- `ITreasuryApiService`
- `IDistributedCache`
- loggers

This keeps tests fast and focused on the behavior under test.

## Running Unit Tests

```bash
dotnet test tests/Wex.Purchases.UnitTests/Wex.Purchases.UnitTests.csproj
```

## Related Documentation

- [Testing Strategy](testing-strategy.md)
- [Integration Tests](integration-tests.md)
