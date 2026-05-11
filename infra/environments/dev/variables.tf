variable "aws_region" {
  description = "AWS region for the development environment."
  type        = string
}

variable "project_name" {
  description = "Project name used as the naming prefix."
  type        = string
  default     = "wex-purchases"
}

variable "vpc_cidr" {
  description = "CIDR block for the VPC."
  type        = string
}

variable "public_subnet_cidrs" {
  description = "CIDRs for public subnets."
  type        = list(string)
}

variable "private_app_subnet_cidrs" {
  description = "CIDRs for private application subnets."
  type        = list(string)
}

variable "private_data_subnet_cidrs" {
  description = "CIDRs for private data subnets."
  type        = list(string)
}

variable "allowed_ingress_cidrs" {
  description = "CIDRs allowed to reach the ALB."
  type        = list(string)
  default     = ["0.0.0.0/0"]
}

variable "alb_certificate_arn" {
  description = "Optional ACM certificate ARN for HTTPS."
  type        = string
  default     = null
}

variable "db_username" {
  description = "Master username for MySQL."
  type        = string
}

variable "db_name" {
  description = "Application database name."
  type        = string
  default     = "wex_purchases"
}

variable "db_instance_class" {
  description = "RDS instance class."
  type        = string
}

variable "db_allocated_storage" {
  description = "Allocated RDS storage in GiB."
  type        = number
}

variable "db_max_allocated_storage" {
  description = "Maximum autoscaled storage in GiB."
  type        = number
}

variable "redis_node_type" {
  description = "ElastiCache node type."
  type        = string
}

variable "redis_num_cache_clusters" {
  description = "Number of Redis nodes."
  type        = number
}

variable "ecs_desired_count" {
  description = "Desired number of ECS tasks."
  type        = number
}

variable "ecs_task_cpu" {
  description = "Task CPU units."
  type        = number
}

variable "ecs_task_memory" {
  description = "Task memory in MiB."
  type        = number
}

variable "ecs_enable_autoscaling" {
  description = "Enable autoscaling for ECS service."
  type        = bool
  default     = true
}

variable "ecs_autoscaling_min_capacity" {
  description = "Minimum task count for autoscaling."
  type        = number
  default     = 1
}

variable "ecs_autoscaling_max_capacity" {
  description = "Maximum task count for autoscaling."
  type        = number
  default     = 2
}

variable "container_image_tag" {
  description = "Bootstrap image tag used by the initial ECS task definition."
  type        = string
  default     = "latest"
}

variable "alarm_actions" {
  description = "Optional alarm action ARNs."
  type        = list(string)
  default     = []
}
