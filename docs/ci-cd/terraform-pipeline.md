# Terraform Pipeline

Terraform is currently organized as infrastructure code in the repository and can be executed manually or through a dedicated pipeline stage.

## Current Model

The application CI/CD workflows deploy application images to already-provisioned ECS services. Terraform is responsible for creating and maintaining those services and supporting AWS resources.

Recommended sequence:

1. Provision or update infrastructure with Terraform.
2. Export Terraform outputs into GitHub repository variables and secrets.
3. Run GitHub Actions application pipelines to publish images and update ECS services.

## Terraform Execution Flow

```bash
cd infra/environments/dev
cp terraform.tfvars.example terraform.tfvars
terraform init
terraform fmt -check
terraform validate
terraform plan
terraform apply
```

Repeat for production from:

```bash
infra/environments/prod
```

## Pipeline Stages

| Stage | Purpose |
| --- | --- |
| Format | Ensures consistent Terraform formatting with `terraform fmt -check`. |
| Init | Downloads providers and initializes backend configuration. |
| Validate | Verifies Terraform configuration syntax and provider schema compatibility. |
| Plan | Shows proposed infrastructure changes before apply. |
| Apply | Applies approved infrastructure changes. |
| Output | Captures values needed by CI/CD workflows. |

## State Management

The repository does not hardcode a backend because backend resources are account-specific.

Recommended production backend:

- S3 bucket for Terraform state
- DynamoDB or equivalent locking mechanism
- separate state path per environment
- encryption enabled
- restricted IAM access

Example state layout:

```text
s3://company-terraform-state/wex-purchases/dev/terraform.tfstate
s3://company-terraform-state/wex-purchases/prod/terraform.tfstate
```

## GitHub Actions Inputs From Terraform

After `terraform apply`, use outputs to configure GitHub variables:

| GitHub Variable | Source |
| --- | --- |
| `AWS_REGION` | Terraform output `aws_region` |
| `AWS_ACCOUNT_ID` | Terraform output `aws_account_id` |
| `ECR_REPOSITORY` | Terraform output `ecr_repository_name` |
| `ECS_CLUSTER_DEVELOP` | Dev Terraform ECS cluster output |
| `ECS_SERVICE_DEVELOP` | Dev Terraform ECS service output |
| `ECS_TASK_DEFINITION_DEVELOP` | Dev Terraform task definition output |
| `ECS_CLUSTER_PRODUCTION` | Prod Terraform ECS cluster output |
| `ECS_SERVICE_PRODUCTION` | Prod Terraform ECS service output |
| `ECS_TASK_DEFINITION_PRODUCTION` | Prod Terraform task definition output |

## First Deployment Consideration

The Terraform examples default ECS desired count to `0` to allow infrastructure creation before the first Docker image exists in ECR.

Recommended first-deploy flow:

1. Apply Terraform with `desired_count = 0`.
2. Run the application pipeline to push the first Docker image.
3. Update environment variables or Terraform variables to the desired count.
4. Apply Terraform again.
5. Let GitHub Actions perform subsequent image deployments.

## Related Documentation

- [Terraform](../infra/terraform.md)
- [GitHub Actions](github-actions.md)
- [Deployment Flow](deployment-flow.md)
