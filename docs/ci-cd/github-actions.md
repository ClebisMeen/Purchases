# GitHub Actions

The repository contains two CI/CD workflows:

| Workflow | File | Trigger |
| --- | --- | --- |
| Develop | `.github/workflows/ci-develop.yml` | Push and pull request to `develop`. |
| Main | `.github/workflows/ci-main.yml` | Push to `main`. |

Both workflows build the .NET solution, execute tests, validate formatting, build a Docker image, push it to Amazon ECR, and deploy to Amazon ECS when appropriate.

The branch workflows are orchestrators. Shared job implementations live in `.github/workflows/ci-shared-*.yml` reusable workflows and are invoked through `workflow_call`.

## Develop Workflow

The develop workflow is optimized for fast validation and continuous deployment to the development ECS service.

| Job | Purpose |
| --- | --- |
| `build` | Restores and builds application, unit test, and integration test projects with warnings as errors. |
| `tests` | Runs unit and integration test suites and uploads TRX artifacts. |
| `format` | Runs `dotnet format --verify-no-changes`. |
| `package` | Builds and pushes `develop-{sha}` and `develop-latest` Docker image tags to ECR. |
| `deploy` | Registers a new ECS task definition revision and updates the development ECS service. |

The `package` and `deploy` jobs run only for pushes to `develop`.

## Main Workflow

The main workflow is the production release path.

| Job | Purpose |
| --- | --- |
| `build` | Produces release builds with warnings as errors. |
| `tests` | Runs unit and integration tests with coverage collection. |
| `coverage` | Parses Cobertura coverage files and enforces `MINIMUM_COVERAGE`. |
| `format` | Validates formatting for application and test projects. |
| `package` | Builds and pushes `prod-{sha}` and `latest` Docker image tags to ECR. |
| `deploy` | Updates ECS production, waits for stability, performs a health check, and rolls back on health-check failure. |

## OIDC Authentication

Both workflows use:

```yaml
permissions:
  id-token: write
```

The AWS credential step assumes an IAM role through GitHub OIDC. This avoids static AWS access keys in repository secrets.

Environment-specific role secrets:

| Environment | Secret |
| --- | --- |
| Development | `AWS_ROLE_TO_ASSUME_DEVELOP` |
| Production | `AWS_ROLE_TO_ASSUME_PRODUCTION` |

## Docker Image Tags

| Branch | Immutable Tag | Moving Tag |
| --- | --- | --- |
| `develop` | `develop-${GITHUB_SHA}` | `develop-latest` |
| `main` | `prod-${GITHUB_SHA}` | `latest` |

The immutable tag is used for traceable deployments. The moving tag is useful for discovery and quick environment inspection.

## Artifacts

The workflows upload:

- unit test results
- integration test results
- deployment state before and after ECS updates
- rendered ECS task definition JSON

Artifacts help diagnose failures without requiring direct access to runner machines.

## Stability Validation

Development deploy waits for ECS service stability.

Production deploy adds:

- post-deploy health check using `APP_HEALTHCHECK_URL`
- automatic rollback to the previous task definition when health check fails
- final failure marking after rollback

## Related Documentation

- [Deployment Flow](deployment-flow.md)
- [Terraform Pipeline](terraform-pipeline.md)
- [Environments](environments.md)
