# Environments

The project separates development and production concerns across Terraform, GitHub Actions, and runtime configuration.

## Environment Matrix

| Environment | Branch | Workflow | Terraform Path | ASP.NET Core Environment |
| --- | --- | --- | --- | --- |
| Development | `develop` | `ci-develop.yml` | `infra/environments/dev` | `Development` |
| Production | `main` | `ci-main.yml` | `infra/environments/prod` | `Production` |

## Development

Development is optimized for frequent deployment and lower infrastructure cost.

Traits:

- ALB deletion protection disabled
- RDS Multi-AZ disabled
- shorter backup and log retention
- single NAT gateway configuration
- ECS deployment after successful develop branch validation

## Production

Production is optimized for durability and safer releases.

Traits:

- ALB deletion protection enabled
- RDS Multi-AZ enabled
- RDS deletion protection enabled
- longer backup and log retention
- production health check after deployment
- automatic rollback on health-check failure

## GitHub Environments

The workflows target GitHub environments named:

- `development`
- `production`

Production can be configured with approval gates, protected branch rules, and restricted secret access.

## Related Documentation

- [GitHub Actions](github-actions.md)
- [Terraform](../infra/terraform.md)
- [AWS Architecture](../architecture/aws-architecture.md)
