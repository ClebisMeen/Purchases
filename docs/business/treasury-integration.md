# Treasury Integration

The API integrates with the U.S. Treasury Fiscal Data API to retrieve exchange rates for purchase conversion.

## Endpoint

Base URL:

```text
https://api.fiscaldata.treasury.gov
```

Rates endpoint:

```text
/services/api/fiscal_service/v1/accounting/od/rates_of_exchange
```

## Query Strategy

The API filters by:

| Filter | Purpose |
| --- | --- |
| `country_currency_desc:eq:{description}` | Matches the supported country currency. |
| `record_date:lte:{purchaseDate}` | Ensures the rate is not after the purchase date. |
| `record_date:gte:{purchaseDateMinusSixMonths}` | Enforces the six-month product rule. |

The query sorts by newest record first:

```text
sort=-record_date
page[size]=1
```

This returns the most recent valid exchange rate.

## Supported Mappings

| API Country Code | Treasury `country_currency_desc` |
| --- | --- |
| `BR` | `Brazil-Real` |
| `CA` | `Canada-Dollar` |
| `MX` | `Mexico-Peso` |

## Resilience

The Treasury client uses:

- `HttpClientFactory`
- Polly retry policy
- Polly timeout policy
- retry support for transient HTTP failures
- retry support for HTTP 429 Too Many Requests

The retry policy performs three attempts with exponential backoff.

## No-Rate Scenario

If Treasury returns no matching data, the application raises an error:

```text
No exchange rate was found for '{currency}' on or before '{purchaseDate}'.
```

This is required by the product brief.

## Related Documentation

- [Business Rules](business-rules.md)
- [Application Flow](../architecture/application-flow.md)
- [Non-Functional Requirements](non-functional-requirements.md)
