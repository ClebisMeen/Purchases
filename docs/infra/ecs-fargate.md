# ECS Fargate

Amazon ECS Fargate runs the Wex Purchases API container without requiring EC2 host management.

## Runtime Model

| Component | Purpose |
| --- | --- |
| ECS cluster | Logical runtime boundary for services. |
| ECS service | Keeps the desired number of API tasks running. |
| Task definition | Defines container image, CPU, memory, port mappings, secrets, environment variables, and logging. |
| Fargate task | Isolated running instance of the API container. |
| Target group | Connects ALB traffic to healthy ECS tasks. |

## Container Settings

The API listens on port `8080`:

```text
ASPNETCORE_URLS=http://+:8080
```

The ALB target group uses `/health` for health checks.

## Secrets

Connection strings are injected into the ECS task from Secrets Manager:

- `ConnectionStrings__PurchaseDb`
- `ConnectionStrings__Redis`

This prevents database and cache credentials from being stored in plain text in the task definition.

## Autoscaling

The ECS module supports autoscaling by CPU and memory targets.

| Environment | CPU Target | Memory Target |
| --- | --- | --- |
| Development | 70% | 75% |
| Production | 65% | 70% |

## Related Documentation

- [AWS Architecture](../architecture/aws-architecture.md)
- [Deployment Flow](../ci-cd/deployment-flow.md)
- [Observability](observability.md)
