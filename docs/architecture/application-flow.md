# Application Flow

This document describes the runtime flow for creating purchases and retrieving converted purchases.

## Create Purchase Flow

```mermaid
sequenceDiagram
    participant Client
    participant API as PurchaseController
    participant App as PurchaseService
    participant Validator as CreatePurchaseRequestValidator
    participant Domain as Money/PurchaseTransaction
    participant Repo as IPurchaseRepository
    participant Db as MySQL

    Client->>API: POST /api/purchases
    API->>App: CreateAsync(request)
    App->>Validator: Validate request
    Validator-->>App: Valid
    App->>Domain: Round amount and create transaction
    App->>Repo: AddAsync(transaction)
    Repo->>Db: Insert purchase
    Db-->>Repo: Saved
    Repo-->>App: Complete
    App-->>API: CreatePurchaseResult
    API-->>Client: 200 OK
```

## Currency Conversion Flow

```mermaid
sequenceDiagram
    participant Client
    participant API as PurchaseController
    participant Cache as Redis Decorator
    participant App as PurchaseService
    participant Repo as IPurchaseRepository
    participant Db as MySQL
    participant Treasury as TreasuryApiService
    participant Fiscal as Treasury API

    Client->>API: GET /api/purchases/{id}?countryCode=BR
    API->>Cache: GetByIdAsync(request)
    Cache->>Cache: Validate and build cache key
    alt Cache hit
        Cache-->>API: Converted result from cache
    else Cache miss
        Cache->>App: GetByIdAsync(request)
        App->>Repo: GetByIdAsync(id)
        Repo->>Db: Query purchase
        Db-->>Repo: Purchase
        App->>Treasury: GetExchangeRateAsync(currency, purchaseDate)
        Treasury->>Fiscal: Filter date window and currency
        Fiscal-->>Treasury: Most recent valid rate
        Treasury-->>App: Exchange rate
        App->>App: Convert and round amount
        App-->>Cache: Converted result
        Cache->>Cache: Store short-lived cache entry
        Cache-->>API: Converted result
    end
    API-->>Client: 200 OK
```

## Treasury Rate Selection

The Treasury request filters rates by:

- `country_currency_desc:eq:{currencyDescription}`
- `record_date:lte:{purchaseDate}`
- `record_date:gte:{purchaseDate minus 6 months}`

The query sorts by descending `record_date` and requests one item:

```text
sort=-record_date
page[size]=1
```

This returns the most recent rate less than or equal to the purchase date within the allowed six-month window.

## Error Flow

| Scenario | Behavior |
| --- | --- |
| Invalid create request | Validation exception handled by global exception middleware. |
| Invalid country code | Validation exception with accepted values. |
| Purchase not found | Service returns `null`; API returns the serialized result from the controller contract. |
| No Treasury rate found | Application throws an error explaining no exchange rate was found. |
| Treasury transient failure | Polly retries transient failures and timeout failures. |
| Treasury permanent failure | HTTP exception is surfaced through global exception handling. |

## Related Documentation

- [Business Rules](../business/business-rules.md)
- [Treasury Integration](../business/treasury-integration.md)
- [Clean Architecture](clean-architecture.md)
