# Docker Compose

Docker Compose provides the standard local runtime for the Wex Purchases API. It starts the application and required infrastructure without manually installing MySQL or Redis.

## Services

| Service | Container | Image or Build | Port |
| --- | --- | --- | --- |
| `wex-purchases-api` | `wex-purchases-api` | Built from `Dockerfile` | `8080:8080` |
| `wex-mysql` | `wex-purchases-mysql` | `mysql:8.4` | `3306:3306` |
| `wex-redis` | `wex-purchases-redis` | `redis:latest` | `6379:6379` |

## Start the Stack

```bash
docker compose up --build
```

Verify health:

```bash
curl http://localhost:8080/health
```

Open Swagger:

```text
http://localhost:8080/swagger
```

## Stop the Stack

```bash
docker compose down
```

Remove the MySQL volume:

```bash
docker compose down -v
```

Use volume removal when you want to recreate the database from scratch.

## Startup Order

The API depends on:

- MySQL being healthy.
- Redis being started.

MySQL uses a `mysqladmin ping` health check. Once MySQL is healthy, the API starts and applies EF Core migrations automatically.

## Persistence

MySQL data is stored in the named volume:

```text
wex_mysql_data
```

This keeps local purchase data available across container restarts.

## Configuration

The API container receives container-network connection strings:

```yaml
ConnectionStrings__PurchaseDb: server=wex-mysql;port=3306;database=wex_purchases;user=wex;password=wex123
ConnectionStrings__Redis: wex-redis:6379
```

These differ from the source-run local defaults, which use `localhost`.

## Related Documentation

- [Local Development](local-development.md)
- [Environment Variables](environment-variables.md)
- [Testing Strategy](../testing/testing-strategy.md)
