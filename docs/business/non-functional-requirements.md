# Non-Functional Requirements

The API is designed to demonstrate production-grade engineering practices appropriate for a cloud-hosted .NET service.

## Requirements

| Category | Requirement | Implementation |
| --- | --- | --- |
| Maintainability | Clear separation of concerns | Clean Architecture project structure. |
| Testability | Business logic must be testable without real infrastructure | Interfaces, fakes, Moq, WebApplicationFactory, EF InMemory. |
| Reliability | External API calls should tolerate transient failures | Polly retry and timeout policies. |
| Portability | Local runtime should be reproducible | Dockerfile and Docker Compose. |
| Deployability | Application should be deployable through CI/CD | GitHub Actions workflows and ECS deployment jobs. |
| Security | Cloud deployments should avoid static AWS keys | GitHub Actions OIDC and IAM roles. |
| Observability | Runtime should expose health and logs | `/health`, CloudWatch Logs, CloudWatch alarms. |
| Scalability | Application runtime should support horizontal scaling | ECS Fargate service and stateless API containers. |
| Performance | Repeated converted reads should avoid unnecessary work | Redis distributed cache. |
| Infrastructure consistency | Cloud resources should be reproducible | Terraform modules and environment stacks. |

## Quality Gates

| Gate | Purpose |
| --- | --- |
| Build with warnings as errors | Prevents warning drift. |
| Unit tests | Validates domain and application behavior. |
| Integration tests | Validates HTTP and application composition behavior. |
| Formatting validation | Keeps code style consistent. |
| Coverage gate on main | Enforces minimum test coverage for production releases. |
| ECS stability wait | Ensures service scheduler reaches stable state. |
| Production health check | Confirms deployed API responds after rollout. |

## Related Documentation

- [GitHub Actions](../ci-cd/github-actions.md)
- [Testing Strategy](../testing/testing-strategy.md)
- [Observability](../infra/observability.md)
