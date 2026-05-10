# Functional Requirements

The functional requirements are based on the Wex technical challenge product brief.

## Requirements

| ID | Requirement | Implementation |
| --- | --- | --- |
| FR-001 | Store purchase transactions. | `POST /api/purchases` persists purchase data in MySQL through `IPurchaseRepository`. |
| FR-002 | Retrieve purchase transactions converted to another currency. | `GET /api/purchases/{id}?countryCode={code}` returns converted purchase data. |
| FR-003 | Use Treasury Reporting Rates of Exchange API. | `TreasuryApiService` calls the Fiscal Data rates endpoint. |
| FR-004 | Use an exchange rate less than or equal to the purchase date. | Treasury query applies `record_date:lte:{purchaseDate}`. |
| FR-005 | Use only rates within the last six months from the purchase date. | Treasury query applies `record_date:gte:{purchaseDate - 6 months}`. |
| FR-006 | Return an error when no exchange rate exists. | Application throws an error when Treasury returns no valid rate. |
| FR-007 | Round currency conversion. | `Money` rounds to two decimal places. |
| FR-008 | Provide production-grade architecture. | Clean Architecture, DI, resilience policies, containerization, CI/CD, and Terraform. |
| FR-009 | Provide automated tests. | Unit and integration test projects are included. |
| FR-010 | Run without manually installing databases. | Docker Compose starts MySQL and Redis. |

## API Operations

| Operation | Method | Route |
| --- | --- | --- |
| Create purchase | `POST` | `/api/purchases` |
| Get converted purchase | `GET` | `/api/purchases/{id}?countryCode={code}` |
| Health check | `GET` | `/health` |

## Related Documentation

- [Business Rules](business-rules.md)
- [Non-Functional Requirements](non-functional-requirements.md)
- [Treasury Integration](treasury-integration.md)
