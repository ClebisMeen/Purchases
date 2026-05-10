# Terraform

Terraform defines the AWS infrastructure for the Wex Purchases API.

## Structure

```text
infra/
├── environments
│   ├── dev
│   └── prod
└── modules
    ├── alb
    ├── ecr
    ├── ecs
    ├── elasticache-redis
    ├── rds-mysql
    └── vpc
```

## Modules

| Module | Responsibility |
| --- | --- |
| `vpc` | VPC, public subnets, private application subnets, private data subnets, internet gateway, NAT gateway. |
| `ecr` | Container image repository and lifecycle policy. |
| `alb` | Application Load Balancer, target group, listener, security group, alarms. |
| `ecs` | ECS cluster, service, task definition, IAM roles, logs, autoscaling, alarms. |
| `rds-mysql` | MySQL database, subnet group, security group, backups, monitoring settings. |
| `elasticache-redis` | Redis cache, subnet group, security group, snapshots, alarms. |

## Environment Separation

Development and production share modules but differ in settings.

Production enables stronger durability defaults such as:

- RDS Multi-AZ
- deletion protection
- longer backup retention
- longer CloudWatch log retention
- ALB deletion protection

## Variables

Each environment defines variables for:

- AWS region
- project name
- VPC CIDR and subnet CIDRs
- ALB ingress CIDRs
- certificate ARN
- database size and credentials
- Redis node type and cluster count
- ECS CPU, memory, desired count, and autoscaling limits
- container image tag
- alarm actions

Use `terraform.tfvars.example` as the starting point for environment-specific values.

## State Management

No remote backend is hardcoded because backend resources vary by AWS account.

Production recommendation:

- S3 backend
- state encryption
- DynamoDB locking or equivalent
- separate state per environment
- restricted IAM permissions

## Apply Flow

```bash
cd infra/environments/dev
cp terraform.tfvars.example terraform.tfvars
terraform init
terraform plan
terraform apply
```

Use `infra/environments/prod` for production.

## Related Documentation

- [AWS Architecture](../architecture/aws-architecture.md)
- [Terraform Pipeline](../ci-cd/terraform-pipeline.md)
- [Networking](networking.md)
