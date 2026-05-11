# Terraform Infrastructure - Wex Purchases API

Esta pasta contem a infraestrutura AWS do projeto em Terraform, organizada por ambientes e modulos reutilizaveis.

## Estrutura

```text
infra/
  environments/
    dev/
    prod/
  modules/
    vpc/
    ecr/
    alb/
    ecs/
    rds-mysql/
    elasticache-redis/
```

## Recursos provisionados

- VPC com subnets publicas, privadas de aplicacao e privadas de dados
- Internet Gateway e NAT Gateway
- Amazon ECR
- Application Load Balancer
- Amazon ECS Fargate
- Amazon RDS MySQL
- Amazon ElastiCache Redis
- AWS Secrets Manager para connection strings da aplicacao
- CloudWatch Log Group
- CloudWatch Alarms basicos para ALB, ECS e RDS

## Observacao importante sobre o primeiro deploy

O ECS Service e criado com `desired_count = 0` por padrao nos exemplos de `terraform.tfvars.example`.

Isso foi feito para permitir o provisionamento da infraestrutura antes de a imagem da aplicacao existir no ECR.

Fluxo recomendado:

1. Aplicar o Terraform com `desired_count = 0`
2. Publicar a primeira imagem no ECR via GitHub Actions
3. Ajustar `desired_count` no ambiente para `1` em `dev` e para o valor desejado em `prod`
4. Executar `terraform apply` novamente

## Como usar

Exemplo para desenvolvimento:

```bash
cd infra/environments/dev
cp terraform.tfvars.example terraform.tfvars
terraform init
terraform plan
terraform apply
```

Exemplo para producao:

```bash
cd infra/environments/prod
cp terraform.tfvars.example terraform.tfvars
terraform init
terraform plan
terraform apply
```

## Backend remoto

O backend remoto nao foi hardcoded para evitar acoplamento com uma conta especifica.

Recomendacao enterprise:

- backend `s3`
- lock com DynamoDB ou mecanismo equivalente
- bucket separado por conta/ambiente

## Outputs importantes para GitHub Actions

Depois do `apply`, os ambientes exportam outputs uteis para configurar os workflows:

- `aws_region`
- `aws_account_id`
- `ecr_repository_name`
- `ecr_repository_url`
- `ecs_cluster_name`
- `ecs_service_name`
- `ecs_task_definition_family`
- `alb_dns_name`
- `application_url`

## Seguranca

- Segredos nao ficam hardcoded nos workflows
- Connection strings sao armazenadas no Secrets Manager
- ALB fica em subnet publica
- ECS, RDS e Redis ficam em subnets privadas
- RDS e Redis so aceitam trafego do security group da aplicacao
- ECS usa IAM Role dedicada para execution e task
