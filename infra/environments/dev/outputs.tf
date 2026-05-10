output "aws_region" {
  description = "AWS region used by the environment."
  value       = var.aws_region
}

output "aws_account_id" {
  description = "Current AWS account ID."
  value       = data.aws_caller_identity.current.account_id
}

output "ecr_repository_name" {
  description = "ECR repository name."
  value       = module.ecr.repository_name
}

output "ecr_repository_url" {
  description = "ECR repository URL."
  value       = module.ecr.repository_url
}

output "ecs_cluster_name" {
  description = "ECS cluster name."
  value       = module.ecs.cluster_name
}

output "ecs_service_name" {
  description = "ECS service name."
  value       = module.ecs.service_name
}

output "ecs_task_definition_family" {
  description = "ECS task definition family."
  value       = module.ecs.task_definition_family
}

output "ecs_container_name" {
  description = "ECS container name."
  value       = module.ecs.container_name
}

output "alb_dns_name" {
  description = "ALB DNS name."
  value       = module.alb.dns_name
}

output "application_url" {
  description = "Base URL of the application."
  value       = var.alb_certificate_arn == null ? "http://${module.alb.dns_name}" : "https://${module.alb.dns_name}"
}

output "purchase_db_connection_secret_arn" {
  description = "Secret ARN storing the PurchaseDb connection string."
  value       = aws_secretsmanager_secret.purchase_db_connection.arn
}

output "redis_connection_secret_arn" {
  description = "Secret ARN storing the Redis connection string."
  value       = aws_secretsmanager_secret.redis_connection.arn
}
