variable "name" {
  description = "Base name used for ECS resources."
  type        = string
}

variable "aws_region" {
  description = "AWS region used by the ECS task logging configuration."
  type        = string
}

variable "subnet_ids" {
  description = "Private application subnet IDs used by the ECS service."
  type        = list(string)
}

variable "target_group_arn" {
  description = "ALB target group ARN."
  type        = string
}

variable "security_group_ids" {
  description = "Security groups attached to the ECS service."
  type        = list(string)
}

variable "container_name" {
  description = "Container name inside the task definition."
  type        = string
}

variable "container_port" {
  description = "Application container port."
  type        = number
}

variable "container_image" {
  description = "Container image used by the task definition."
  type        = string
}

variable "task_cpu" {
  description = "Fargate task CPU units."
  type        = number
}

variable "task_memory" {
  description = "Fargate task memory in MiB."
  type        = number
}

variable "desired_count" {
  description = "Desired task count for the ECS service."
  type        = number
}

variable "assign_public_ip" {
  description = "Whether to assign public IPs to tasks."
  type        = bool
  default     = false
}

variable "health_check_grace_period_seconds" {
  description = "Grace period before ALB health checks start counting against tasks."
  type        = number
  default     = 60
}

variable "log_retention_days" {
  description = "Log retention period for the application log group."
  type        = number
  default     = 30
}

variable "environment_variables" {
  description = "Non-secret environment variables injected into the container."
  type        = map(string)
  default     = {}
}

variable "secret_arns" {
  description = "Map of environment variable names to Secrets Manager ARNs."
  type        = map(string)
  default     = {}
}

variable "enable_autoscaling" {
  description = "Whether autoscaling is enabled for the ECS service."
  type        = bool
  default     = false
}

variable "autoscaling_min_capacity" {
  description = "Minimum number of tasks for autoscaling."
  type        = number
  default     = 1
}

variable "autoscaling_max_capacity" {
  description = "Maximum number of tasks for autoscaling."
  type        = number
  default     = 2
}

variable "cpu_target_value" {
  description = "Target average CPU utilization for autoscaling."
  type        = number
  default     = 70
}

variable "memory_target_value" {
  description = "Target average memory utilization for autoscaling."
  type        = number
  default     = 75
}

variable "alarm_actions" {
  description = "Alarm actions for ECS alarms."
  type        = list(string)
  default     = []
}

variable "tags" {
  description = "Tags applied to ECS resources."
  type        = map(string)
  default     = {}
}
