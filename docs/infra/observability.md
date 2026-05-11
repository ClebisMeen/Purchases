# Observability

The project includes baseline observability for application health, deployment stability, and AWS runtime signals.

## Health Check

The API exposes:

```text
GET /health
```

This endpoint is used by:

- local smoke tests
- Docker verification
- ALB target group health checks
- production post-deploy validation

## CloudWatch Logs

ECS sends container logs to CloudWatch Logs. Terraform configures log retention per environment:

| Environment | Retention |
| --- | --- |
| Development | 30 days |
| Production | 90 days |

## CloudWatch Alarms

Terraform modules define baseline alarms for platform components such as:

- ALB unhealthy hosts
- ECS service metrics
- RDS metrics
- Redis metrics

Alarm actions are configurable through Terraform variables.

## Deployment Observability

GitHub Actions deployment jobs collect:

- ECS service state before deployment
- rendered task definition
- ECS service state after deployment
- recent ECS service events

Production also writes health-check and rollback summaries to the GitHub Actions step summary.

## Related Documentation

- [Deployment Flow](../ci-cd/deployment-flow.md)
- [GitHub Actions](../ci-cd/github-actions.md)
- [AWS Architecture](../architecture/aws-architecture.md)
