# Business Rules

This document defines the core purchase and currency-conversion rules implemented by the API.

## Purchase Creation

| Rule | Description |
| --- | --- |
| Description required | A purchase must include a non-empty description. |
| Description length | Description must be at most 50 characters. |
| Transaction date required | A purchase must include a valid transaction date. |
| Positive amount | Purchase amount must be greater than zero. |
| Two decimal places | Purchase amount must have at most two decimal places. |
| USD storage | Purchases are stored as USD amounts. |
| Rounding | Monetary values are normalized to two decimal places. |

## Currency Conversion

| Rule | Description |
| --- | --- |
| Purchase must exist | Conversion starts from a stored purchase transaction. |
| Country code required | The caller must provide a supported `countryCode`. |
| Supported countries | `BR`, `CA`, and `MX` are supported. |
| Treasury rate source | Exchange rates come from the Treasury Reporting Rates of Exchange API. |
| Date rule | The rate must have `record_date` less than or equal to the purchase date. |
| Six-month window | The rate must be no older than six months before the purchase date. |
| No rate found | The API returns an error when no valid rate exists. |
| Converted rounding | Converted amounts are rounded to two decimal places. |

## Supported Countries

| Country Code | Country | Treasury Currency Description |
| --- | --- | --- |
| `BR` | Brazil | `Brazil-Real` |
| `CA` | Canada | `Canada-Dollar` |
| `MX` | Mexico | `Mexico-Peso` |

## Precision

The `Money` value object rounds using:

```csharp
decimal.Round(amount, 2, MidpointRounding.AwayFromZero)
```

This rule applies to stored purchase amounts and converted values.

## Related Documentation

- [Functional Requirements](functional-requirements.md)
- [Treasury Integration](treasury-integration.md)
- [Application Flow](../architecture/application-flow.md)
