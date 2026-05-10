# Environment Variables

The API uses ASP.NET Core configuration binding. Values can come from `appsettings.json`, environment-specific settings, Docker Compose variables, AWS Secrets Manager, or runtime environment variables.

## Application Settings

| Key | Required | Example | Purpose |
| --- | --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | `Development` | Selects runtime environment and enables Swagger in development. |
| `ASPNETCORE_URLS` | Yes in containers | `http://+:8080` | Configures the HTTP listener inside the container. |
| `ConnectionStrings__PurchaseDb` | Yes | `server=wex-mysql;port=3306;database=wex_purchases;user=wex;password=wex123` | MySQL connection string used by EF Core. |
| `ConnectionStrings__Redis` | Yes | `wex-redis:6379` | Redis endpoint used by distributed cache. |
| `TreasuryApi__BaseUrl` | Yes | `https://api.fiscaldata.treasury.gov` | Base URL for the Treasury Fiscal Data API. |
| `TreasuryApi__RatesOfExchangeEndpoint` | Yes | `/services/api/fiscal_service/v1/accounting/od/rates_of_exchange` | Treasury rates endpoint path. |
| `TreasuryApi__TimeoutSeconds` | Yes | `10` | Polly timeout policy duration. |
| `TreasuryApi__HttpClientTimeoutSeconds` | Yes | `100` | HttpClient timeout setting. |

## Local Defaults

The default local settings are defined in `src/Wex.Purchases.Api/appsettings.json`.

```json
{
  "ConnectionStrings": {
    "PurchaseDb": "server=localhost;port=3306;database=wex_purchases;user=wex;password=wex123",
    "Redis": "localhost:6379"
  }
}
```

Docker Compose overrides these values so containers communicate by service name instead of localhost.

## Docker Compose Variables

`docker-compose.yml` configures the API container with:

| Variable | Value |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Development` |
| `ASPNETCORE_URLS` | `http://+:8080` |
| `ConnectionStrings__PurchaseDb` | `server=wex-mysql;port=3306;database=wex_purchases;user=wex;password=wex123` |
| `ConnectionStrings__Redis` | `wex-redis:6379` |

## AWS Runtime Configuration

Terraform injects non-secret environment variables into the ECS task definition and stores connection strings in AWS Secrets Manager.

| Configuration | Source |
| --- | --- |
| Treasury API settings | ECS task definition environment variables |
| ASP.NET Core runtime settings | ECS task definition environment variables |
| MySQL connection string | Secrets Manager |
| Redis connection string | Secrets Manager |

Secret names follow this pattern:

```text
/{project_name}/{environment}/ConnectionStrings/PurchaseDb
/{project_name}/{environment}/ConnectionStrings/Redis
```

## Security Guidance

- Do not commit real production connection strings.
- Use AWS Secrets Manager for production secrets.
- Use GitHub Actions OIDC for AWS authentication instead of static access keys.
- Keep Terraform variable files containing sensitive values out of source control.

## Related Documentation

- [Local Development](local-development.md)
- [Docker Compose](docker-compose.md)
- [Terraform](../infra/terraform.md)
- [Environments](../ci-cd/environments.md)
