# Docker Compose

O Docker Compose fornece o runtime local padrao da Wex Purchases API. Ele sobe a aplicacao e a infraestrutura usada no desenvolvimento sem exigir instalacao manual de MySQL, Redis, SonarQube, PostgreSQL ou Datadog Agent no host.

## Servicos

| Servico | Container | Imagem ou build | Porta |
| --- | --- | --- | --- |
| `wex-purchases-api` | `wex-purchases-api` | Build local a partir do `Dockerfile` | `8080:8080` |
| `wex-mysql` | `wex-purchases-mysql` | `mysql:8.4` | `3306:3306` |
| `wex-redis` | `wex-purchases-redis` | `redis:latest` | `6379:6379` |
| `sonarqube` | `wex-purchases-sonarqube` | `sonarqube:community` | `9000:9000` |
| `sonarqube-db` | `wex-purchases-sonarqube-db` | `postgres:16-alpine` | Apenas rede interna |
| `datadog-agent` | `wex-purchases-datadog-agent` | `gcr.io/datadoghq/agent:latest` | `8126:8126` |

## Subir o ambiente

```bash
docker compose up --build
```

Validar a saude da API:

```bash
curl http://localhost:8080/health
```

Abrir o Swagger:

```text
http://localhost:8080/swagger
```

Abrir o SonarQube:

```text
http://localhost:9000
```

## Parar o ambiente

```bash
docker compose down
```

Remover tambem os volumes locais:

```bash
docker compose down -v
```

Use a remocao de volumes quando quiser recriar do zero os dados locais do MySQL, SonarQube e PostgreSQL.

## Ordem de inicializacao

A API depende de:

- MySQL saudavel.
- Redis iniciado.
- Datadog Agent iniciado.

O MySQL usa um health check com `mysqladmin ping`. Depois que o MySQL fica saudavel, a API inicia e aplica as migrations do EF Core automaticamente.

O SonarQube depende do `sonarqube-db`, um PostgreSQL dedicado usado apenas pela instancia local do SonarQube.

## SonarQube

O servico `sonarqube` executa o SonarQube Community Edition para analise local de qualidade de codigo. Ele permite validar issues, security hotspots, duplicacao, cobertura e Quality Gate antes de promover alteracoes.

Ele fica disponivel em `http://localhost:9000` e usa o login local padrao `admin / admin` no primeiro acesso. O SonarQube pode solicitar a troca da senha inicial.

O SonarQube nao usa o MySQL da aplicacao. Ele usa o servico `sonarqube-db`, baseado em PostgreSQL, com a connection string:

```yaml
SONAR_JDBC_URL: jdbc:postgresql://sonarqube-db:5432/sonarqube
SONAR_JDBC_USERNAME: sonarqube
SONAR_JDBC_PASSWORD: sonarqube
```

Para subir somente o SonarQube e o banco dele:

```bash
docker compose up -d sonarqube sonarqube-db
```

Guia detalhado: [SonarQube](../ci-cd/sonarqube.md).

## Datadog Agent

O servico `datadog-agent` executa o Agent local da Datadog. Ele coleta logs de containers, traces APM, metricas de runtime da API .NET, metricas DogStatsD e telemetria dos containers Docker.

A API envia traces para o Agent pela rede interna do Compose usando:

```yaml
DD_AGENT_HOST: datadog-agent
DD_TRACE_AGENT_PORT: 8126
DD_TRACE_ENABLED: "true"
DD_LOGS_INJECTION: "true"
DD_RUNTIME_METRICS_ENABLED: "true"
```

O Agent tambem le os logs dos containers por meio das montagens do Docker e das configuracoes:

```yaml
DD_LOGS_ENABLED: "true"
DD_LOGS_CONFIG_CONTAINER_COLLECT_ALL: "true"
DD_APM_ENABLED: "true"
DD_APM_NON_LOCAL_TRAFFIC: "true"
DD_DOGSTATSD_NON_LOCAL_TRAFFIC: "true"
```

Para enviar dados para a Datadog, crie um arquivo `.env` na raiz do repositorio com a API key da sua conta:

```env
DD_API_KEY=YOUR_DATADOG_API_KEY
```

Opcionalmente ajuste o site e as tags padrao:

```env
DD_SITE=datadoghq.com
DD_ENV=development
DD_SERVICE=wex-purchases
DD_VERSION=1.0.0
```

As labels `com.datadoghq.*` nos servicos ajudam o Agent a atribuir logs e tags aos servicos corretos no Datadog.

Guia detalhado: [Datadog Observability](../observability/datadog.md).

## Persistencia

Os dados locais sao armazenados em volumes nomeados:

```text
wex_mysql_data
sonarqube_data
sonarqube_extensions
sonarqube_logs
sonarqube_db_data
```

Esses volumes preservam dados entre restarts dos containers. Use `docker compose down -v` apenas quando quiser apagar esse estado local.

## Configuracao da API

A API recebe connection strings usando nomes de servicos da rede do Compose:

```yaml
ConnectionStrings__PurchaseDb: server=wex-mysql;port=3306;database=wex_purchases;user=wex;password=wex123
ConnectionStrings__Redis: wex-redis:6379
```

Esses valores diferem dos defaults usados ao rodar a API direto no host, que normalmente apontam para `localhost`.

## Documentacao relacionada

- [Local Development](local-development.md)
- [Environment Variables](environment-variables.md)
- [SonarQube](../ci-cd/sonarqube.md)
- [Datadog Observability](../observability/datadog.md)
- [Testing Strategy](../testing/testing-strategy.md)
