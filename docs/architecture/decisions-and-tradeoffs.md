# Decisions and Tradeoffs

This document captures the main technical decisions behind the Wex Purchases API.

## Summary

| Decision | Rationale | Tradeoff |
| --- | --- | --- |
| Clean Architecture | Keeps business rules independent from HTTP, database, and cloud details. | More projects and dependency wiring. |
| EF Core for persistence | Provides migrations, mapping, and transactional write support. | Higher abstraction overhead than direct SQL. |
| Dapper as future read optimization | Useful for high-volume read models and tuned SQL if the API grows. | Should not be introduced until query complexity justifies it. |
| Redis cache | Reduces repeated converted-read work and external Treasury calls. | Adds operational dependency and cache invalidation considerations. |
| Docker Compose | Satisfies no-manual-database setup and standardizes local runtime. | Requires Docker locally. |
| ECS Fargate | Runs containers without managing EC2 hosts. | Less host-level control than EC2. |
| Terraform | Makes AWS infrastructure reproducible and reviewable. | Requires state management discipline. |
| GitHub Actions | Keeps build and deployment automation close to the repository. | Tightly coupled to GitHub as the source platform. |
| AWS OIDC | Avoids static AWS keys in GitHub. | Requires IAM trust policy setup. |

## EF Core and Dapper

The current code uses EF Core for repository persistence because the data model is small, transactional, and migration-driven.

Dapper is documented as the read-path option for future scenarios such as:

- reporting endpoints
- joins across read models
- high-volume lookup paths
- SQL that must be tuned explicitly

Introducing Dapper before those needs exist would add complexity without measurable benefit.

## Redis Cache

The conversion endpoint can repeatedly request the same purchase and country code. Redis caches that result briefly.

Benefits:

- reduces duplicate Treasury requests
- lowers latency for repeated reads
- supports horizontal ECS scaling better than in-memory cache

Tradeoff:

- cached converted values may remain briefly stale if upstream Treasury data changes.

The short expiration window intentionally limits that risk.

## ECS Fargate

Fargate is a good fit for an API challenge project because it demonstrates production container deployment without the operational overhead of Kubernetes or EC2 cluster management.

## Terraform Environment Split

Reusable modules live under `infra/modules`, while environment-specific compositions live under `infra/environments/dev` and `infra/environments/prod`.

This lets both environments share architecture while changing durability and lifecycle settings such as deletion protection, backup retention, Multi-AZ, and log retention.

## Related Documentation

- [Clean Architecture](clean-architecture.md)
- [AWS Architecture](aws-architecture.md)
- [Terraform](../infra/terraform.md)
